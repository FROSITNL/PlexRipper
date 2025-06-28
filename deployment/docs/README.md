PlexRipper - alternative setup docs
===================================

These docs describe alternative setup methods.

- Dockerless / standalone binary's
    - Linux
    - Windows
    - MacOS
- Home Assistant

In short, this guide explains how to build PlexRipper yourself for __almost any architecture__ with alternative parameters. This would allow for custom implementations of PlexRipper, for example, hosted, integrations, distribution.

This guide covers the following main topics.

1. Dependency's / requirements
2. environment configuration

# Build Requirements

- WebAPI: dotnet8-sdk
- WebAPP: nodejs (18 - 22)
    - npm
    - bun (otional)

## Setting up dotnet

There are multiple ways forward, though I would recommend either the dotnet-cli-installer or using package management.

```
RUN apt-get update && \
    apt-get install -y wget apt-transport-https && \
    wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-runtime-8.0 unzip wget
```


### Runtime

To run self-contained executables, some runtime depende3nfies are needed:

- https://github.com/dotnet/core/blob/main/release-notes/8.0/linux-packages.md
- https://github.com/dotnet/core/blob/main/release-notes/8.0/install.md


## Setting up node

@TODO



# Building the WebAPI

We need to run dotnet build or publish on src/WebApi

### Modes

See:
- https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?tabs=cli


dotnet build: builds project
dotnet publish: builds and exports it to dfir

```
# Release config
-c Release 

# https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
-r / --runtime

# @TODO describe
-p:UseAppHost=false

# platform specific executable
# no native libraries included
--self-contained

# makes them large
-p:PublishReadyToRun=true

# specifies current rid 
--use-current-runtime, --ucr 

-o tmp
```

Framework-dependent deployment: `dotnet publish -c Release -p:UseAppHost=false`
Framework-dependent executable: `dotnet publish -c Release -r <RID> --self-contained false`
Self-contained deployment: `dotnet publish -c Release -r <RID> --self-contained true`


