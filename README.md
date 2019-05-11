# CytubeBot-.NET

A chat bot for Cytube using .NET Core

This is still in the very early stages of the bot.

## Installation

There is two ways to install this

### Visual Studio
1. Clone the repo
2. Open the solution in VS
3. Right click the CytubeBotCore project and click "Properties"
4. Go to tab Debug and add following Environment variables:  
server:Host  
server:Channel  
server:Username  
server:Password
5. Save (ctrl + s)
6. Debugg (F5)

### Command line
TODO


## Usage

### To add custom commands

Make a new class in commands folder with the filename as the command name and extend the class "Command"

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
