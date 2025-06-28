TO DO
=====

Things we need to do to handle this properly.



## Modify Assembly path loading code

This breaks with self-contained deploys


## Implement ConfigurationManager properly

The static environment functions don't do cli arg fallback. 



## ASSEmlby

```
/share/plexripper-git/src/Environment/PathProvider.cs(71,45): warning IL3000: 'System.Reflection.Assembly.Location' always returns an empty string for assemblies embedded in a single-file app. If the path to the app directory is needed, consider calling 'System.AppContext.BaseDirectory'. [/share/plexripper-git/src/Environment/Environment.csproj]

/share/plexripper-git/src/PlexApi/Common/Mappers/MediaContainer/MediaContainerMappers.GetLibraryItemsMetadata.cs(36,37): warning CS8601: Possible null reference assignment. [/share/plexripper-git/src/PlexApi/PlexApi.csproj]
/share/plexripper-git/src/PlexApi/Common/Mappers/MediaContainer/MediaContainerMappers.GetMediaMetaDataMetadata.cs(36,37): warning CS8601: Possible null reference assignment. [/share/plexripper-git/src/PlexApi/PlexApi.csproj]

/share/plexripper-git/src/WebAPI/Startup/Startup.Application.cs(20,35): warning IL3000: 'System.Reflection.Assembly.Location' always returns an empty string for assemblies embedded in a single-file app. If the path to the app directory is needed, consider calling 'System.AppContext.BaseDirectory'. [/share/plexripper-git/src/WebAPI/WebAPI.csproj]
/share/plexripper-git/src/WebAPI/Startup/Startup.Services.cs(98,39): warning IL3000: 'System.Reflection.Assembly.Location' always returns an empty string for assemblies embedded in a single-file app. If the path to the app directory is needed, consider calling 'System.AppContext.BaseDirectory'. [/share/plexripper-git/src/WebAPI/WebAPI.csproj]
```


## Formatting

```
    The file contained different line endings than formatting it would result in.
  Error /share/plexripper-git/src/WebAPI/Startup/Startup.Base.cs - Was not formatted.
    The file contained different line endings than formatting it would result in.
  Error /share/plexripper-git/src/WebAPI/Startup/Startup.Services.cs - Was not formatted.
    ----------------------------- Expected: Around Line 101 -----------------------------
                );

                if (env.IsDevelopment())
    ----------------------------- Actual: Around Line 101 -----------------------------
                );
    ············
                if (env.IsDevelopment())

  Error /share/plexripper-git/src/WebAPI/Config/Autofac/WebApiModule.cs - Was not formatted.
  ```

  