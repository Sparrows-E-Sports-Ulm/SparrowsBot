FROM ubuntu:latest

# Install DOTNET SDK
RUN apt-get update && apt-get install -y wget dotnet-sdk-7.0 sudo git

# Setup user
RUN useradd -ms /bin/bash -p "$(openssl passwd -6 sparrows)" dev
RUN usermod -aG sudo dev
WORKDIR /home/dev

USER dev

