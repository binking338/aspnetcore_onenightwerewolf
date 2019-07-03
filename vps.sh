#!/usr/bin/env bash 

# 准备环境

# dotnet runtime
sudo rpm -Uvh https://packages.microsoft.com/config/rhel/7/packages-microsoft-prod.rpm
yum update -y
yum install -y dotnet-runtime-2.2
yum install -y aspnetcore-runtime-2.2

# nvm
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.34.0/install.sh | bash
source /root/.bashrc

# node
nvm install v10.16.0

# pm2
npm install pm2 -g



