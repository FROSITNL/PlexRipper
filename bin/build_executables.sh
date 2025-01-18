#!/usr/bin/env bash
# =====================
# This script helps building executables for various platforms.
# ------------------------------------------------------------
# TO DO
# -----
# * testing / finetuning
# * releasing
# * documentation
# * CI
#
#
#
# 
#
# ===============


_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
_dir_project="$(cd "$_dir/.." && pwd)"
_dir_tmp="$_dir_project/tmp"

OUTPUT_DIR="$_dir_project/releases"
APP_NAME="PlexRipper"
VERSION=$(git branch --show-current)

# FLags

VERBOSE=0
CONFIG=Release
INSTALL=0

# Detect OS and CPU architecture
OS=$(uname -s)
ARCH=$(uname -m)

arg=$1
shift

help() {
    echo "Usage: $0 [options] [command]"
    echo "Options:"
    echo "  --verbose, -v    Enable verbose output"
    echo "  --config=CONFIG  Build configuration (default: Release)"
    echo "  --install        Install required dependencies"
    echo "  --version=V0..   Install required dependencies"
    echo "Commands:"
    echo "  build            Build executables for various platforms"
    echo "  clean            Clean up build artifacts"
    echo "  help             Show this help message"
    exit 1
}

if [[ -z "$arg" ]]; then
    help
fi

while [[ "$#" -gt 0 ]]; do
    case $1 in
    --verbose | -v)
        VERBOSE=1
        shift
        ;;
    --version=*)
        VERSION="${1#*=}"
        ;;        
    --config=*) CONFIG="${1#*=}" ;;
    --install)
        INSTALL="$2"
        shift
        ;;
    *)
        echo "Unknown parameter passed: $1"
        exit 1
        ;;
    esac
    shift
done

# @TODO if verbose
echo "Detected OS: $OS"
echo "Detected Architecture: $ARCH"

## ====================== Environment preparation ====================

check_requirements() {
    if ! command -v dotnet &>/dev/null; then
        echo ".NET SDK not found. Exiting."
        exit 1
    fi
}

# Define helper functions
install_dotnet() {
    echo "Checking for .NET SDK..."
    if ! command -v dotnet &>/dev/null; then
        echo ".NET SDK not found. Installing .NET SDK version 8..."
        curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0
        

        #@TODO handle this better
        export DOTNET_ROOT=$HOME/.dotnet
        export PATH=$HOME/.dotnet:$PATH


        echo ".NET SDK installed successfully."
    else
        INSTALLED_VERSION=$(dotnet --version)
        if [[ $INSTALLED_VERSION != 8.* ]]; then
            echo "Found .NET SDK version $INSTALLED_VERSION. Updating to version 8..."
            curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0
            echo ".NET SDK updated to version 8."
        else
            echo ".NET SDK version 8 is already installed."
        fi
    fi
}

install_linux_dependencies() {
    echo "Checking for required Linux packages..."
    if [[ "$ARCH" == "x86_64" || "$ARCH" == "i686" ]]; then
        if ! dpkg -l | grep -q gcc-multilib; then
            echo "Installing gcc-multilib and g++-multilib..."
            sudo apt update && sudo apt install -y gcc-multilib g++-multilib
            echo "Multilib packages installed."
        else
            echo "Multilib packages already installed."
        fi
    fi
}

install_macos_dependencies() {
    echo "Checking for required macOS packages..."
    if ! command -v brew &>/dev/null; then
        echo "Homebrew not found. Installing Homebrew..."
        /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
        echo "Homebrew installed successfully."
    fi

    echo "Checking for required macOS packages with Homebrew..."
    if ! brew list --formula | grep -q cmake; then
        echo "Installing cmake..."
        brew install cmake
    else
        echo "cmake is already installed."
    fi
}

function install() {
    case $OS in
    Linux)
        install_dotnet
        install_linux_dependencies
        ;;
    Darwin)
        install_dotnet
        install_macos_dependencies
        ;;
    *)
        echo "Unsupported OS: $OS. Exiting."
        exit 1
        ;;
    esac
}

##################
# Cleanup dirtree
# @TODO finish this
function clean() {
    echo "Cleaning up..."
    exit 0
    rm -rf "$_dir_project/bin"
    rm -rf "$_dir_project/obj"
    rm -rf "$_dir_project/tmp"
    echo "Cleanup complete."
}

################
# Build dotnet app
# ----------------
#
################
function build() {

    BUILD_DIR="$_dir_tmp/$VERSION"

    if [[ ! -d $BUILD_DIR ]]; then
        mkdir -p $BUILD_DIR
    else
        echo -e "Directory $BUILD_DIR already exists.!!"
        # exit 1
    fi

    mkdir -p $OUTPUT_DIR

    declare -a TARGETS
    if [[ "$ARCH" == "x86_64" ]]; then
        TARGETS=("linux-x64" "win-x64" "osx-x64")
    elif [[ "$ARCH" == "aarch64" ]]; then
        TARGETS=("linux-arm64" "win-arm64" "osx-arm64" "linux-arm")
    else
        echo "Unsupported architecture: $ARCH"
        exit 1
    fi

    # Iterate all targets and build
    for TARGET in "${TARGETS[@]}"; do
        TARGET_BUILD_DIR="$BUILD_DIR/$APP_NAME-$VERSION-$TARGET"

        echo "Building for $TARGET... ($TARGET_BUILD_DIR)"

        dotnet publish \
            "src/WebAPI/WebAPI.csproj" \
            -r $TARGET \
            -c $CONFIG \
            /p:AssemblyVersion=$VERSION \
            /p:PublishTrimmed=false \
            /p:PublishSingleFile=true \
            --self-contained true \
            -o "$TARGET_BUILD_DIR"

        if [[ $? -ne 0 ]]; then
            echo "Build failed for $TARGET. Exiting."
            exit 1
        else
            echo -e "Build succeeded for $TARGET_BUILD_DIR"
        fi

        echo "Packaging $TARGET_BUILD_DIR..."
        cd "$OUTPUT_DIR" || exit
        zip -r "$APP_NAME-$VERSION-$TARGET.zip" "$APP_NAME-$VERSION-$TARGET"
        cd - || exit

        echo "Packaged: $APP_NAME-$VERSION-$TARGET.zip"

    done
}

${arg%%/} $@
