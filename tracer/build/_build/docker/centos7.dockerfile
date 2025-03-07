﻿FROM gleocadie/centos7-clang16 as base

ARG DOTNETSDK_VERSION

# Based on https://github.com/dotnet/dotnet-docker/blob/34c81d5f9c8d56b36cc89da61702ccecbf00f249/src/sdk/6.0/bullseye-slim/amd64/Dockerfile
# and https://github.com/dotnet/dotnet-docker/blob/1eab4cad6e2d42308bd93d3f0cc1f7511ac75882/src/sdk/5.0/buster-slim/amd64/Dockerfile
ENV \
    # Unset ASPNETCORE_URLS from aspnet base image
    ASPNETCORE_URLS= \
    # Do not generate certificate
    DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
    # Do not show first run text
    DOTNET_NOLOGO=true \
    # Enable correct mode for dotnet watch (only mode supported in a container)
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    # Skip extraction of XML docs - generally not useful within an image/container - helps performance
    NUGET_XMLDOC_MODE=skip \
    # Disable LTTng tracing with QUIC
    QUIC_LTTng=0

RUN yum update -y \
    && yum install -y centos-release-scl \
    && yum install -y\
        git \
        procps \
        wget \
        curl \
        gcc \
        build-essential \
        rpm \
        ruby \
        ruby-devel \
        rubygems \
        uuid-dev \
        autoconf \
        libtool \
        libstdc++ \
        libstdc++-devel \
        liblzma-dev \
        gdb \
        libicu \
        which \
        zlib \
        zlib-devel \
        devtoolset-7 \
        rpm-build \
        expect \
        sudo \
        gawk \
        libasan6 \
        libubsan1

# Install newer version of fpm and specific version of dotenv 
RUN echo "gem: --no-document --no-rdoc --no-ri" > ~/.gemrc && \
    gem install --version 1.12.2 --user-install ffi && \
    gem install --version 1.6.0 --user-install git && \
    gem install --version 0.9.10 --user-install rb-inotify && \
    gem install --version 3.2.3  --user-install rexml && \
    gem install backports -v 3.21.0 && \
    gem install --version 2.7.6 dotenv && \
    gem install --version 1.14.2 --minimal-deps fpm

RUN curl -Ol https://raw.githubusercontent.com/llvm-mirror/clang-tools-extra/master/clang-tidy/tool/run-clang-tidy.py \
    && mv run-clang-tidy.py /usr/bin/ \
    && chmod +x /usr/bin/run-clang-tidy.py \ 
    && ln -s /usr/bin/run-clang-tidy.py /usr/bin/run-clang-tidy

# Install CppCheck
RUN curl -sSL https://download-ib01.fedoraproject.org/pub/epel/7/x86_64/Packages/c/cppcheck-2.7-1.el7.x86_64.rpm --output cppcheck-2.7-1.el7.x86_64.rpm \
    && sudo yum localinstall -y cppcheck-2.7-1.el7.x86_64.rpm

# Install the .NET SDK
RUN curl -sSL https://dot.net/v1/dotnet-install.sh --output dotnet-install.sh  \
    && chmod +x ./dotnet-install.sh \
    && ./dotnet-install.sh --version $DOTNETSDK_VERSION --install-dir /usr/share/dotnet \
    && rm ./dotnet-install.sh \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
# Trigger first run experience by running arbitrary cmd
    && dotnet help

ENV CXX=clang++
ENV CC=clang

FROM base as builder

# Copy the build project in and build it
COPY . /build
RUN dotnet build /build
WORKDIR /project

FROM base as tester

# Install ASP.NET Core runtimes using install script
# There is no arm64 runtime available for .NET Core 2.1, so just install the .NET Core runtime in that case

RUN if [ "$(uname -m)" = "x86_64" ]; \
    then export NETCORERUNTIME2_1=aspnetcore; \
    else export NETCORERUNTIME2_1=dotnet; \
    fi \
    && curl -sSL https://dot.net/v1/dotnet-install.sh --output dotnet-install.sh \
    && chmod +x ./dotnet-install.sh \
    && ./dotnet-install.sh --runtime $NETCORERUNTIME2_1 --channel 2.1 --install-dir /usr/share/dotnet --no-path \
    && ./dotnet-install.sh --runtime aspnetcore --channel 3.0 --install-dir /usr/share/dotnet --no-path \
    && ./dotnet-install.sh --runtime aspnetcore --channel 3.1 --install-dir /usr/share/dotnet --no-path \
    && ./dotnet-install.sh --runtime aspnetcore --channel 5.0 --install-dir /usr/share/dotnet --no-path \
    && ./dotnet-install.sh --runtime aspnetcore --channel 6.0 --install-dir /usr/share/dotnet --no-path \
    && rm dotnet-install.sh


# Copy the build project in and build it
COPY . /build
RUN dotnet build /build
WORKDIR /project
