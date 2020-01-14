# Youtube Video Subscription Downloader
A straightforward service built in .NetCore (v3.0) that'll scan through a subscription.xml file generated from YouTube
and download the latest videos every 6 hours.

## How to use
1. Install .NetCore v3 SDK.
2. In root directory, run:

```
dotnet publish -c Release -o "publish/"
```
3. Be sure to update the "appsettings.json" config values for a relevant "Download" folder, and where the app can find a "subscription" xml file.

4. Execute resulting artifact (VideoSubscriptionsSaver for Linux, VideoSubscriptionsSaver.exe for Windows) in the publish folder.