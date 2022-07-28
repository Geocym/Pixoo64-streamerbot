# Pixoo64-streamerbot
c# code for passing commands from twitch and youtube chat to your Divoom Pixoo64 via streamerbot


the pixoo commands v5 file is your main import code

it will add a !pv5 command to your streamerbot setup, everything tags off that


!pv5 gif <url to a gif>               //send an online gif to the device, use only 16x16, 32x32 or 64x64 gifs or it will hang
!pv5 buzzer                           //play a series of beeps
!pv5 channel <0-3>                    //change the display mode 0=Faces(clock), 1=Cloud(community gif chanenel), 2=Sound Visualiser 3=Custom
!pv5 clock                            //show the clock
!pv5 vis                              //show the sound visualiser
!pv5 cycle                            //show the cluod gif channel
!pv5 countdown <number of minutes>    //will do 1 min if omited>
!pv5 timerstart                       //start a stopwatch
!pv5 timerstop                        //stop the stopwatch
!pv5 timerreset                       //reset the stopwatch
!pv5 score <red score> <blue score>   //show a 2 team score board with values given
!pv5 noisestart                       //show a decibel meter
!pv5 noisestop                        //freeze the decibel meter (must run this before restarting decibel meter if you switch to another mode)
!pv5 brightness <0-100>               //set the brightness
!pv5 weather <longtitude> <latitude>  //set the location for weather widget
!pv5 timezone <timezone string>       //set the timezone
!pv5 on                               //turn on the screen
!pv5 off                              //turn off the screen
!pv5 c                                //set temperature mode to celcius
!pv5 f                                //set temperature mode to farenheit
!pv5 rotate <0-3>                     //set rotation of the screen (0:normal, 1:90, 2:180, 3:270)
!pv5 mirror <0-1>                     //set horizontal flip mode(0:normal, 1:flipped)
!pv5 hours <0-1>                      //set 12/24h clock mode (0:12h mode, 1:24h mode)
!pv5 overdrive <0-1>                  //set high brightness mode (warning make sure you can drive 5V 3A or it will crash)
!pv5 white <red strength> <green strength> <blue strength>  //set the white balance per colour chanel

