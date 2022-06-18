#!/bin/bash
DISTRO=""

NatInstall() {
	echo "系统版本：$DISTRO"
    if [[ $DISTRO == "CentOS" ]];
    then
        sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
        sudo yum install dotnet-sdk-6.0 -y
    elif [[ $DISTRO == "Debian" ]];
    then
	echo "系统版本：Debian 暂时无法安装"
    elif [[ $DISTRO == "Ubuntu" ]];
    then
    	wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb 
    	sudo dpkg -i packages-microsoft-prod.deb 
    	rm packages-microsoft-prod.deb 
    	sudo apt-get update; \
    	sudo apt-get install -y apt-transport-https && \
    	sudo apt-get update && \
    	sudo apt-get install -y dotnet-sdk-6.0
    fi
}

Get_Dist_Name()
{
    if grep -Eqii "CentOS" /etc/issue || grep -Eq "CentOS" /etc/*-release; then
        DISTRO="CentOS"
    elif grep -Eqi "Red Hat Enterprise Linux Server" /etc/issue || grep -Eq "Red Hat Enterprise Linux Server" /etc/*-release; then
        DISTRO="CentOS"
    elif grep -Eqi "Aliyun" /etc/issue || grep -Eq "Aliyun" /etc/*-release; then
        DISTRO="Aliyun"
    elif grep -Eqi "Fedora" /etc/issue || grep -Eq "Fedora" /etc/*-release; then
        DISTRO="Fedora"
    elif grep -Eqi "Debian" /etc/issue || grep -Eq "Debian" /etc/*-release; then
        DISTRO="Debian"
    elif grep -Eqi "Ubuntu" /etc/issue || grep -Eq "Ubuntu" /etc/*-release; then
        DISTRO="Ubuntu"
    elif grep -Eqi "Raspbian" /etc/issue || grep -Eq "Raspbian" /etc/*-release; then
        DISTRO="Raspbian"
    else
        DISTRO="unknow"
    fi
}

Install(){
    rm -rf /root/EthereumForward.zip
    rm -rf /root/EthereumForward
    wget https://cdn.jsdelivr.net/gh/weiai-weiai/EthereumForward@master/bin/Debug/EthereumForward.zip -O /root/EthereumForward.zip
    cd /root
    unzip EthereumForward.zip -d EthereumForward
}

Get_Dist_Name

NatInstall

Install