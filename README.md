<div align="center">
<h1>StockportGovUK.AspNetCore.Analytics.Google</h1>
</div>

<div align="center">
  <strong>Track events within Google Analytics with common methods.</strong>
</div>

<br />

<div align="center">
  <sub>Built with :heart: by
  <a href="https://www.stockport.gov.uk">Stockport Council</a>
</div>

## Table of Contents
- [Table of Contents](#table-of-contents)
- [Purpose](#purpose)
- [Installation](#installation)
- [Usage](#usage)
- [Methods](#methods)
  - [TrackEvent](#trackevent)
- [License](#license)

## Purpose
This package exposes a common set of methods which will allow you to fire events into a Google Analytics environment from any .Net Core 2+ application.

Simply register the class in your startup and then using dependancy injection gain access to the methods within any file.

## Installation

Install package using your tool of preference

Package manager
```cmd
PM> Install-Package StockportGovUK.AspNetCore.Attributes.TokenAuthentication
```

.NET CLI
```bash
$ dotnet add package StockportGovUK.AspNetCore.Analytics.Google
```

Package reference
```xml
<PackageReference Include="StockportGovUK.AspNetCore.Attributes.TokenAuthentication" Version="0.2.0" />
```

Paket CLI
```bash
$ paket add StockportGovUK.AspNetCore.Attributes.TokenAuthentication
```

## Usage

Add your tracking code, customer id and data source into your appsetting.json

```json
{
    "Analytics": {
      "TrackingCode": "UA-XXXXXXXXX-X",
      "CustomerId": "555",
      "DataSource": "application"
    }
}
```

Register the package in your startup

```c#
using StockportGovUK.AspNetCore.Analytics.Google;

public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IEventTrackingHelper, EventTrackingHelper>();
}
```

Pass the interface to a class you wish to trigger events with dependacy injection

```c#
public class ValuesController : ControllerBase
{
    private readonly IEventTrackingHelper _eventTrackingHelper;

    public ValuesController(IEventTrackingHelper eventTrackingHelper)
    {
        _eventTrackingHelper = eventTrackingHelper;
    }
}
```

## Methods

### TrackEvent

TrackEvent allows you to send over a Category, Action, Label and Value to Google Analytics.

Label and Value are both optional arguments.

```c#
_eventTrackingHelper.TrackEvent("Category", "Action");
_eventTrackingHelper.TrackEvent("Category", "Action", "Label");
_eventTrackingHelper.TrackEvent("Category", "Action", "Label", 1);
```

## License
[MIT](./src/License.txt)