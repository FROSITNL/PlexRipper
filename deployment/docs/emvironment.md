Environment & configuration
===========================

PlexRipper was build to run inside a docker container, while leaner deployments are possible.
Due to the QEMU multi-builds issues with dotnet, PlexRipper is not available on docker for each arch.

This document describes the parameters to configure PlexRipper for running outside of a docker environment.

There's a spaghetti of (environment) specific configuration options which we should defenitely cleanup at some point but for now I just made it worse to make it work.

There are a few areas of Env vars specific to our frameworks (asp.net/vue)

* DOTNET_
    * ENVIRONMENT
    * URLS
* ASPNETCORE_
    * URLS
* NUXT_
    * HOST
    * PORT
    * PUBLIC_API_PORT
    * IS_DOCKER

These should be documented at the docs of the corresponding framework.



* DEVELOPMENT_ROOT_PATH
* VERSION: semantic version
* PUID=1000
* PGID=1000
* LOG_LEVEL
    * 0=debug
    * ....
    * 5=information
* LOG_ENV_VARS=false
* UNMASKED
