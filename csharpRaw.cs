using System;
using System.Web;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

public class CPHInline
{
    public string FindPixoo()
    {
        //Pixoo64 AutoFinder - this code block searches your local LAN for pixoo devices and returns the IP address of the first one 
        string pixooParams = string.Empty;
        string finderurl = @"https://app.divoom-gz.com/Device/ReturnSameLANDevice";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(finderurl);
        request.AutomaticDecompression = DecompressionMethods.GZip;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    pixooParams = reader.ReadToEnd();
                }

        CPH.LogInfo(pixooParams);
        var finderResult = JObject.Parse(pixooParams);
        string pixooReturn = string.Empty;
        JArray deviceList = (JArray)finderResult["DeviceList"];
        CPH.LogInfo("Number of Pixoo devices found : " + deviceList.Count.ToString());
        if (deviceList.Count == 0)
        {
            CPH.SendMessage("No Pixoo Device Found");
            return pixooReturn;
        }

        CPH.LogInfo(deviceList[0]["DevicePrivateIP"].ToString());
        pixooReturn = deviceList[0]["DevicePrivateIP"].ToString();
        return pixooReturn;
    }

    public bool Execute()
    {
        // your main code goes here
        string pixooIP = FindPixoo();
        //Exit if no Pixoo64 found
        if (pixooIP == string.Empty)
        {
            CPH.SendMessage("Exiting...");
            return false;
        }

        var url = $"http://{pixooIP}:80/post";
        string command = null;
        string input = null;
        // check for input
        bool commandExists = args.ContainsKey("input0");
        bool inputExists = args.ContainsKey("input1");
        bool input2Exists = args.ContainsKey("input2");
        bool input3Exists = args.ContainsKey("input3");
        if (!commandExists)
        {
            CPH.SendMessage("Command not Entered");
            return false;
        }

        //int trimLength = args["input0"].ToString().Length +1;
        string rawInput = args["rawInput"].ToString();
        //string trimmedInput = rawInput.Remove(0,trimLength);
        // setup generic payload objects
        command = args["input0"].ToString();
        var payload = new JObject();
        // add command specific data
        switch (command)
        {
            case "gif": //gifs must be exactly 16x16, 32x32 or 64x64 pixels, anything else will hang
                //default gif for testing if no url entered
                input = @"https://raw.githubusercontent.com/marcmerlin/AnimatedGIFs/master/gifs/gifs64/notkept/001.gif";
                if (inputExists)
                {
                    input = args["input1"].ToString(); //if url is entered use this instead
                }

                payload["Command"] = "Device/PlayTFGif";
                payload["FileType"] = 2;
                payload["FileName"] = input;
                //CPH.SendMessage(input);
                break;
            case "text": //text currently broken - this is a multi step process and i havent worked out the api sequence yet
                input = "Lyfesaver is a cutie!";
                //if (inputExists){input = trimmedInput;}
                if (rawInput.Length > 511)
                {
                    input = input.Remove(512);
                }

                //if (trimmedInput.Length > 511){input = input.Remove(512);}
                payload["Command"] = "Draw/SendHttpText";
                payload["TextId"] = 4; //Any number less than 20 - will overwrite matching IDs
                payload["x"] = 0; //the start x position
                payload["y"] = 40; //the start y position
                payload["dir"] = 0; //Direction - 0=Left, 1=Right
                payload["font"] = 4; //0-7 
                payload["TextWidth"] = 56; //17-63 - Text width, point based
                payload["TextString"] = input; //utf8 encoded string, must be less than 512 characters
                payload["speed"] = 10; //if scrolling required, time in ms to next step
                payload["color"] = "#FFFF00"; //string for color hex code
                payload["align"] = 1; //horisontal text alignment, 1=Left, 2=Middle, 3=Right
                break;
            case "cleartext":
                payload["Command"] = "Draw/ClearHttpText";
                break;
            case "buzzer": //plays an alarm sound for specified period
                payload["Command"] = "Device/PlayBuzzer";
                payload["ActiveTimeInCycle"] = 500; //On period for buzzer in ms
                payload["OffTimeInCycle"] = 500; //Off period for buzzer in ms
                payload["PlayTotalTime"] = 3000; //Total time for cycle in ms	
                break;
            case "channel":
                int channel = 0;
                if (inputExists)
                {
                    channel = Int32.Parse(args["input1"].ToString());
                }

                payload["Command"] = "Channel/SetIndex";
                payload["SelectIndex"] = channel; //0-3 - 0=Faces(clock), 1=Cloud(community gif chanenel), 2=Visualiser 3=Custom
                break;
            case "clock": //show the clock chosen in the divoom app				
                payload["Command"] = "Channel/SetIndex";
                payload["SelectIndex"] = 0; //0-3 - 0=Faces(clock), 1=Cloud(community gif chanenel), 2=Visualiser 3=Custom
                break;
            case "vis": //show a spectral analyser of the onboard microphone, style set in he divoom app			
                payload["Command"] = "Channel/SetIndex";
                payload["SelectIndex"] = 2; //0-3 - 0=Faces(clock), 1=Cloud(community gif chanenel), 2=Visualiser 3=Custom
                break;
            case "cycle": //show the current cloud gif channel as set in Divoom app			
                payload["Command"] = "Channel/SetIndex";
                payload["SelectIndex"] = 1; //0-3 - 0=Faces(clock), 1=Cloud(community gif chanenel), 2=Visualiser 3=Custom
                break;
            case "countdown": //Runs a countdown for the specified time, plays an alarm at the end
                int mins = 1;
                if (inputExists)
                {
                    mins = Int32.Parse(args["input1"].ToString());
                }

                payload["Command"] = "Tools/SetTimer";
                payload["Minute"] = mins;
                payload["Second"] = 0;
                payload["Status"] = 1;
                break;
            case "timerstart": //starts a count up stopwatch
                payload["Command"] = "Tools/SetStopWatch";
                payload["Status"] = 1;
                break;
            case "timerstop": //stops the stopwatch
                payload["Command"] = "Tools/SetStopWatch";
                payload["Status"] = 0;
                break;
            case "timerreset": // reset stopwatch to zero
                payload["Command"] = "Tools/SetStopWatch";
                payload["Status"] = 2;
                break;
            case "score": //show a scoreboard with scores up to 999 for red and blue teams
                int red = 0;
                int blue = 0;
                if (inputExists)
                {
                    red = Int32.Parse(args["input1"].ToString());
                }

                if (input2Exists)
                {
                    blue = Int32.Parse(args["input2"].ToString());
                }

                payload["Command"] = "Tools/SetScoreBoard";
                payload["BlueScore"] = blue;
                payload["RedScore"] = red;
                break;
            case "noisestart": //starts the decibel counter
                payload["Command"] = "Tools/SetNoiseStatus";
                payload["NoiseStatus"] = 1;
                break;
            case "noisestop": //freezes the decibel counter
                payload["Command"] = "Tools/SetNoiseStatus";
                payload["NoiseStatus"] = 0;
                break;
            case "brightness": //sets the panel brightness
                int bright = 100;
                if (inputExists)
                {
                    mins = Int32.Parse(args["input1"].ToString()); // 0-100
                }

                payload["Command"] = "Channel/SetBrightness";
                payload["Brightness"] = bright;
                break;
            case "weather": //sets longitude and latitude for weather widget
                string longitude = "0";
                string latitude = "0";
                if (inputExists)
                {
                    longitude = args["input1"].ToString();
                }

                if (input2Exists)
                {
                    latitude = args["input2"].ToString();
                }

                payload["Command"] = "Sys/LogAndLat";
                payload["Longitude"] = longitude;
                payload["Latitude"] = latitude;
                break;
            case "timezone": //sets timezone
                string tz = "GMT";
                if (inputExists)
                {
                    tz = args["input1"].ToString();
                }

                payload["Command"] = "Sys/TimeZone";
                payload["TimeZoneValue"] = tz;
                break;
            case "on": //turns on the screen
                payload["Command"] = "Channel/OnOffScreen";
                payload["OnOff"] = 1;
                break;
            case "off": //turns off the screen
                payload["Command"] = "Channel/OnOffScreen";
                payload["OnOff"] = 0;
                break;
            case "c": //set temperature mode to celcius
                payload["Command"] = "Device/SetDisTempMode";
                payload["OnOff"] = 0;
                break;
            case "f": //set temperature mode to farenheit
                payload["Command"] = "Device/SetDisTempMode";
                payload["OnOff"] = 1;
                break;
            case "rotate": //sets the panel rotation
                int rotate = 0;
                if (inputExists)
                {
                    rotate = Int32.Parse(args["input1"].ToString()); // 0:normal, 1:90, 2:180, 3:270
                }

                payload["Command"] = "Device/SetScreenRotationAngle";
                payload["Mode"] = rotate;
                break;
            case "mirror": //sets the horizontal flip
                int flip = 0;
                if (inputExists)
                {
                    flip = Int32.Parse(args["input1"].ToString()); // 0:normal, 1:flipped
                }

                payload["Command"] = "Device/SetMirrorMode";
                payload["Mode"] = flip;
                break;
            case "hours": //sets the clock to 12/24h mode
                int hours = 0;
                if (inputExists)
                {
                    hours = Int32.Parse(args["input1"].ToString()); // 0:12h mode, 1:24h mode
                }

                payload["Command"] = "Device/SetTime24Flag";
                payload["Mode"] = hours;
                break;
            case "overdrive": //sets the panel to superbright mode
                int od = 0;
                if (inputExists)
                {
                    od = Int32.Parse(args["input1"].ToString()); // 0:normal mode, 1:superbright mode
                }

                payload["Command"] = "Device/SetHighLightMode";
                payload["Mode"] = od;
                break;
            case "white": //sets the colour balance of the panel
                int rval = 100;
                int gval = 100;
                int bval = 100;
                if (inputExists)
                {
                    rval = Int32.Parse(args["input1"].ToString());
                }

                if (input2Exists)
                {
                    gval = Int32.Parse(args["input2"].ToString());
                }

                if (input3Exists)
                {
                    bval = Int32.Parse(args["input3"].ToString());
                }

                payload["Command"] = "Device/SetWhiteBalance";
                payload["RValue"] = rval;
                payload["GValue"] = gval;
                payload["BValue"] = bval;
                break;
			case "test":
                payload["Command"] = "Draw/SendHttpGif";
            default:
                CPH.SendMessage("Command not Recognised");
                return false;
                break;
        }

        //compile payload
        var httpRequest = (HttpWebRequest)WebRequest.Create(url);
        httpRequest.Method = "POST";
        httpRequest.ContentType = "application/json";
        using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
        {
            streamWriter.Write(payload.ToString());
        }

        //push payload to device
        var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
            CPH.LogInfo(result);
        }

        CPH.LogInfo(httpResponse.StatusCode.ToString());
        return true;
    }
}
