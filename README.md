# Sparrows Bot
The official Discord Bot for UULM.LAN.

## Getting Started
For developing and deploying this application you will need ``docker``and ``docker-compose``. 

### Running / Deploying
Simply run ``docker compose up`` from within this directory to build and start everything. 

### Developing
This project uses C# and ASP.NET. The easiest way to contribute to this project is by using docker and VS CODE. After installing the ``Dev Container`` extension for VS CODE you can open this repo as a Dev Container and all necesary dependencies will be setup automaticaly.

- Running the Bot: ``dotnet run --project src/Bot``
- Running the Web Server: ``dotnet run --project src/Server``

### Config
The config folder contains a configuration file for the Development and Production Environment. Here you can also set the token of the discord bot.
Make sure not to track this file with git to avoid publishing your token to github: You can achieve this with the following command: ``git update-index --assume-unchanged src/Bot/config/appsettings.json``. 

### Database
MongoDB is used as the database and is automaticaly launched alongside the main docker container. It is accessible at ``mongodb://localhost:27017``.

## Inviting the Bot
### Permissions
When inviting the bot to your server it requires the following scopes (make sure to set them when generatring the invite link):
- applications.commands
- bot

The bot also needs the following permissions:
- Use Slash Commands