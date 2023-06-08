using System;
using System.Web;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

public class CPHInline
{
    private const string FinderUrl = @"https://app.divoom-gz.com/Device/ReturnSameLANDevice";
    
    public JObject payload = new JObject();

    public string FindPixoo()
    {
        //Pixoo64 AutoFinder - this code block searches your local LAN for pixoo devices and returns the IP address of the first one 
        string pixooParams = GetPixooParams();
        CPH.LogInfo(pixooParams);
        var finderResult = JObject.Parse(pixooParams);
        string pixooReturn = GetPixooIPAddress(finderResult);
        return pixooReturn;
    }

    private string GetPixooParams()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FinderUrl);
        request.AutomaticDecompression = DecompressionMethods.GZip;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    private string GetPixooIPAddress(JObject finderResult)
    {
        string pixooReturn = string.Empty;
        JArray deviceList = (JArray)finderResult["DeviceList"];
        CPH.LogInfo("Number of Pixoo devices found : " + deviceList.Count.ToString());
        if (deviceList.Count == 0)
        {
            CPH.SendMessage("No Pixoo Device Found");
        }
        else
        {
            CPH.LogInfo(deviceList[0]["DevicePrivateIP"].ToString());
            pixooReturn = deviceList[0]["DevicePrivateIP"].ToString();
        }

        return pixooReturn;
    }
    
    public bool PixooPush(string url)
    {
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
    
    public JObject Buzzer(int ActiveTimeInCycle, int OffTimeInCycle, int PlayTotalTime)
    {
    	payload["Command"] = "Device/PlayBuzzer";
        payload["ActiveTimeInCycle"] = ActiveTimeInCycle; // On period for buzzer in ms (500)
        payload["OffTimeInCycle"] = OffTimeInCycle; // Off period for buzzer in ms (500)
        payload["PlayTotalTime"] = PlayTotalTime; // Total time for cycle in ms (3000)
        return payload;	
    }   

    public JObject Channel(int Channel) // 0-3 - 0=Faces(clock), 1=Cloud(community gif chanenel), 2=Visualiser 3=Custom
    {
        payload["Command"] = "Channel/SetIndex";
        payload["SelectIndex"] = Channel; 
        return payload;
    }
    public JObject Clock() // Show the clock
    {
        payload["Command"] = "Channel/SetIndex";
        payload["SelectIndex"] = 0; 
        return payload;
    }

    public JObject Visualiser() // Show the Animated Sound Visualiser
    {
        payload["Command"] = "Channel/SetIndex";
        payload["SelectIndex"] = 2; 
        return payload;
    }

    public JObject Cycle() // Show a rotation of animated gifs
    {
        payload["Command"] = "Channel/SetIndex";
        payload["SelectIndex"] = 1; 
        return payload;
    }

    public JObject Countdown(int Minutes, int Seconds)
    {
        payload["Command"] = "Tools/SetTimer";
        payload["Minute"] = Minutes;
        payload["Second"] = Seconds;
        payload["Status"] = 1; 
        return payload;
    }
    
    public JObject StopWatch(int StopWatchMode) // 0 = Stop Stopwatch, 1 = Start Stopwatch, 2 = Reset Stopwatch
    {
        payload["Command"] = "Tools/SetStopWatch";
        payload["Status"] = StopWatchMode; 
        return payload;
    }

    public JObject ScoreBoard(int RedScore, int BlueScore) // Show a scoreboard
    {
        payload["Command"] = "Tools/SetScoreBoard";
        payload["BlueScore"] = BlueScore;
        payload["RedScore"] = RedScore;
        return payload;
    }

    public JObject DecibelCounterMode(int DecibelCounterMode) // 0 = Freeze Decibel Counter, 1 = Show and unfreeze Decibel counter
    {
        payload["Command"] = "Tools/SetNoiseStatus";
        payload["NoiseStatus"] = DecibelCounterMode;
        return payload;
    }

    public JObject SetBrightness(int Brightness) // Set device brightness
    {
        payload["Command"] = "Channel/SetBrightness";
        payload["Brightness"] = Brightness;
        return payload;        
    }

    public JObject SendGif(string GifUrl) // Send an online gif to the panel, (gifs must be exactly 16x16, 32x32 or 64x64 pixels, anything else will hang)
    {
        payload["Command"] = "Device/PlayTFGif";
        payload["FileType"] = 2;
        payload["FileName"] = GifUrl;
        return payload;
    }

    public JObject SetWeather(string Longitude, string Latitude) // Set the Location for the Weather Widget
    {
        payload["Command"] = "Sys/LogAndLat";
        payload["Longitude"] = Longitude;
        payload["Latitude"] = Latitude;
        return payload;        
    }

    public JObject SetTimeZone(string TimeZone) // Set the TimeZone
    {
        payload["Command"] = "Sys/TimeZone";
        payload["TimeZoneValue"] = TimeZone;
        return payload;
    }

    public  JObject ScreenOn(int ScreenOn) // 0 = Screen Off, 1 = Screen On
    {
        payload["Command"] = "Channel/OnOffScreen";
        payload["OnOff"] = ScreenOn;
        return payload;
    }

    public JObject SetTemperatureMode(int Imperial) // 0 = Celcius, 1 = Farenheit
    {
        payload["Command"] = "Device/SetDisTempMode";
        payload["OnOff"] = Imperial;
        return payload;
    }

    public JObject SetRotateMode(int RotateMode) // 0:normal, 1:90, 2:180, 3:270
    {
        payload["Command"] = "Device/SetScreenRotationAngle";
        payload["Mode"] = RotateMode;
        return payload;
    }

    public JObject SetMirrorMode(int MirrorMode) // 0 = Normal, 1 = Horizontal Flip
    {
        payload["Command"] = "Device/SetMirrorMode";
        payload["Mode"] = MirrorMode;
        return payload;
    }

    public JObject SetClockType(int MilitaryClock) // 0 = 12h clock, 1 = 24h clock
    {
        payload["Command"] = "Device/SetTime24Flag";
        payload["Mode"] = MilitaryClock;
        return payload;
    }

    public JObject SetOverdriveMode(int Overdrive) // 0 = Normal Brightness range, 1 = Superbright mode
    {
        payload["Command"] = "Device/SetHighLightMode";
        payload["Mode"] = Overdrive;
        return payload;
    }

    public JObject SetWhiteBalance(int Red, int Green, int Blue) // Set the white balance of each RGB channel
    {
        payload["Command"] = "Device/SetWhiteBalance";
        payload["RValue"] = Red;
        payload["GValue"] = Green;
        payload["BValue"] = Blue;
        return payload;
    }

    public JObject TextWriter(string Message, int TextTd, int XPos, int YPos, int Direction, int Font, int TextWidth, int Speed, string TextColor, int Alignment) //Currently doesnt work
    {
        if (Message.Length > 511)
        {
            Message = Message.Remove(512);
        }
        payload["Command"] = "Draw/SendHttpText";
        payload["TextId"] = TextTd; //Any number less than 20 - will overwrite matching IDs
        payload["x"] = XPos; //the start x position
        payload["y"] = YPos; //the start y position
        payload["dir"] = Direction; //Direction - 0=Left, 1=Right
        payload["font"] = Font; //0-7 
        payload["TextWidth"] = TextWidth; //17-63 - Text width, point based
        payload["TextString"] = Message; //utf8 encoded string, must be less than 512 characters
        payload["speed"] = Speed; //if scrolling required, time in ms to next step
        payload["color"] = TextColor; //string for color hex code (#FFFF00)
        payload["align"] = Alignment; //horisontal text alignment, 1=Left, 2=Middle, 3=Right
        return payload;
    }

    public JObject ClearText() // Clear the previous text buffer
    {
        payload["Command"] = "Draw/ClearHttpText";
        return payload;
    }

    public bool Execute()
    {
        // your main code goes here
        string pixooIP = FindPixoo();
        //Exit if no Pixoo64 found
        if (string.IsNullOrEmpty(pixooIP))
        {
            CPH.SendMessage("Exiting...");
            return false;
        }

        //Store Pixoo IP
        var url = $"http://{pixooIP}:80/post";
        string command = null;
        string input = null;

        // check for command
        if (!args.ContainsKey("input0"))
        {
            CPH.SendMessage("Command not Entered");
            return false;
        }
        else
        {
        	command = args["input0"].ToString();
        }
        
        bool inputExists = args.ContainsKey("input1");
        bool input2Exists = args.ContainsKey("input2");
        bool input3Exists = args.ContainsKey("input3");

        //int trimLength = args["input0"].ToString().Length +1;
        string rawInput = args["rawInput"].ToString();
        //string trimmedInput = rawInput.Remove(0,trimLength);
        // setup generic payload objects
        
        // var payload = new JObject();
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
				SendGif(input);
                break;
                
            case "text": //text currently broken - this is a multi step process and i havent worked out the api sequence yet
                input = "Lyfesaver is a cutie!";
                //if (inputExists){input = trimmedInput;}
                if (rawInput.Length > 511)
                {
                    input = input.Remove(512);
                }
                TextWriter(input, 4, 0, 40, 0, 4, 56, 10, "#FFFF00", 1);
                break;
                
            case "cleartext":
                payload["Command"] = "Draw/ClearHttpText";
                break;
                
            case "buzzer": //plays an alarm sound for specified period
				int ontime = 500;
                int offtime = 500;
                int totaltime = 3000;
                if (inputExists)
                {
                    ontime = Int32.Parse(args["input1"].ToString());
                }

                if (input2Exists)
                {
                    offtime = Int32.Parse(args["input2"].ToString());
                }

                if (input3Exists)
                {
                    totaltime = Int32.Parse(args["input3"].ToString());
                }
				Buzzer(ontime,offtime,totaltime); //On period(ms), Off Period(ms), Total Time(ms)
                break;
                
            case "channel":
                int channel = 0;
                if (inputExists)
                {
                    channel = Int32.Parse(args["input1"].ToString());
                }
                Channel(channel);
                break;
                
            case "clock": //show the clock chosen in the divoom app				
                Channel(0);
                break;
                
            case "vis": //show a spectral analyser of the onboard microphone, style set in he divoom app			
                Channel(2);
                break;
                
            case "cycle": //show the current cloud gif channel as set in Divoom app			
                Channel(1);
                break;
                
            case "countdown": //Runs a countdown for the specified time, plays an alarm at the end
                int mins = 1;
                int secs = 0;
                if (inputExists)
                {
                    mins = Int32.Parse(args["input1"].ToString());
                }
                Countdown(mins, secs);
                break;
                
            case "timerstart": //starts a count up stopwatch
                StopWatch(1);
                break;
                
            case "timerstop": //stops the stopwatch
                StopWatch(0);
                break;
                
            case "timerreset": // reset stopwatch to zero
                StopWatch(2);
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
                ScoreBoard(red, blue);
                break;
                
            case "noisestart": //starts the decibel counter
                DecibelCounterMode(1);
                break;
                
            case "noisestop": //freezes the decibel counter
                DecibelCounterMode(0);
                break;
                
            case "brightness": //sets the panel brightness
                int bright = 100;
                if (inputExists)
                {
                    mins = Int32.Parse(args["input1"].ToString()); // 0-100
                }
                SetBrightness(bright);
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
                SetWeather(longitude, latitude);
                break;
                
            case "timezone": //sets timezone
                string tz = "GMT";
                if (inputExists)
                {
                    tz = args["input1"].ToString();
                }
                SetTimeZone(tz);
                break;
                
            case "on": //turns on the screen
                ScreenOn(1);
                break;
                
            case "off": //turns off the screen
                ScreenOn(0);
                break;
                
            case "c": //set temperature mode to celcius
                SetTemperatureMode(0);
                break;
                
            case "f": //set temperature mode to farenheit
				SetTemperatureMode(1);
                break;
                
            case "rotate": //sets the panel rotation
                int rotate = 0;
                if (inputExists)
                {
                    rotate = Int32.Parse(args["input1"].ToString()); // 0:normal, 1:90, 2:180, 3:270
                }
                SetRotateMode(rotate);
                break;
                
            case "mirror": //sets the horizontal flip
                int flip = 0;
                if (inputExists)
                {
                    flip = Int32.Parse(args["input1"].ToString()); // 0:normal, 1:flipped
                }
                SetMirrorMode(flip);
                break;
                
            case "hours": //sets the clock to 12/24h mode
                int hours = 0;
                if (inputExists)
                {
                    hours = Int32.Parse(args["input1"].ToString()); // 0:12h mode, 1:24h mode
                }
                SetClockType(hours);
                break;
                
            case "overdrive": //sets the panel to superbright mode
                int od = 0;
                if (inputExists)
                {
                    od = Int32.Parse(args["input1"].ToString()); // 0:normal mode, 1:superbright mode
                }
                payload["Command"] = "Device/SetHighLightMode";
                payload["Mode"] = od;
                SetOverdriveMode(od);
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
                SetWhiteBalance(rval, gval, bval);
                break;

            default:
                CPH.SendMessage("Command not Recognised");
                return false;
                break;
        }

        PixooPush(url);
        return true;
    }
}
