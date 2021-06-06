# WpfTourPlanner Documentation
## Architecture
The Application has a layer based architecture: user interface - business logic - data access. The division into those three layers enables the possibility to easily exchange and modify layers. For example if a terminal "UI" is needed in the future all the other layers could be reused and only the UI layer would need to be changed / newly developed. Another example is if the requirement of a different data access method arises. This layered architecture enables the possibility of easily swapping out the PostgreSQL data access layer for a e.g. SQLite data access layer.
## UX / library decisions
### PDF generation: [QuestPDF](https://www.questpdf.com/)
QuestPDF seemed (and is) easy to use and is open source
### JSON: [Newtonsoft JSON](https://www.newtonsoft.com/json)
### Logging: [log4net](https://logging.apache.org/log4net/)
### Additional WPF Dialog: [Ookii.Dialogs.Wpf](https://github.com/ookii-dialogs/ookii-dialogs-wpf)
I do not like the default Windows 10 folder browser dialog so used this library to use the old folder explorer
### UX
Regarding UX emojis were used to emphasize the function of a button (e.g. 💾 for save)

## Design pattern
Factory design pattern: [Refactoring Guru](https://refactoring.guru/design-patterns/factory-method)
Factories were used to get instances of the respective layers. There is one Factory that returns a business layer implementation and one factory that returns an instance of the implementation. For this project the data access layer and business could have also been a Singleton, however the factory pattern enables the possibility to add additional data access or business layers that then can easily be swapped by providing different parameters to the factory.


## Unit tests


## Unique feature
CI/CD Pipeline using GitHub Actions.
As a unique feature I chose to setup a continues integration and delivery pipeline using GitHub Actions. I wanted to create automatic Releases (compiled binaries) when pushing a new tag to the git repo. Furthermore unit tests should also be executed automatically. With a Windows application package a msix installer is compiled and provided as a zip folder to download. The application is released for both x86 and x64 systems. However due to a bug in .NET 5 and the MSIX packaging tool continues integration (automatic execution of unit tests and creating a msix installer in with Debug configuration) only works for x86, but this is not that big of an issue since x64 platforms can execute x86 applications. For further details regarding this bug see:
Fixed but not released yet: [.NET 5 MSB4044 GitHub Issue](https://github.com/dotnet/sdk/issues/18031)
No answer yet: [MSIX GitHub Issue regarding MSB4044 error](https://github.com/microsoft/MSIX-PackageSupportFramework/issues/180)

## Time Tracking
Time Tracking:
| Date      | time in h | Note                                                                         |
| --        | --        | --                                                                           |
| 7-mar-21  | 2         | Hello World in WPF                                                           |
| 19-mar-21 | 2         | Design thoughs / understanding MVVM                                          |
| 21-mar-21 | 2         | DAL PoC                                                                      |
| 22-mar-21 | 2         | Created UI without bindings                                                  |
| 14-may-21 | 4         | Wpf + layer tutorial                                                         |
| 15-may-21 | 9         | CI/CD Pipeline                                                               |
| 16-may-21 | 6         | CI/CD Pipeline                                                               |
| 17-may-21 | 3         | started from scrath with propper architecture                                |
| 18-may-21 | 3         | Added first bindings to UI                                                   |
| 19-may-21 | 3         | custom UI component                                                          |
| 19-may-21 | 4         | Added bindings for text fields in UI                                         |
| 19-may-21 | 3         | Added copy feature for tours and logs                                        |
| 20-may-21 | 3         | Added update tour and delete tour(log) feature                               |
| 23-may-21 | 1.5       | Added import and export feature                                              |
| 23-may-21 | 1.5       | Added fulltext search                                                        |
| 24-may-21 | 3         | Added report generation                                                      |
| 01-jun-21 | 3         | Added Unit Tests                                                             |
| 03-jun-21 | 1.5       | Added copy and delete for Images                                             |
| 03-jun-21 | 2         | Added basic navigation                                                       |
| 04-jun-21 | 7         | Added Create and edit of TourLog functionality + fixed Image deletion        |
| 05-jun-21 | 5         | Added create tour feature + async api call to mapquest api                   |
| 05-jun-21 | 4         | Fixed msix installer (by including app.config in repo) + wrote documentation |


## Todo
### Mandatory
* ~~Uses markup-Based UI framework~~
* ~~Uses MVVM for UI~~
* ~~Implements a layer-based architecture (UI/BL/DAL)~~
* ~~Implements at least one design pattern~~
* ~~Uses a Postgres Database for storing Tour Data~~
* ~~Does not allow for SQL injection~~
* ~~Does not use an OR-Mapping Library~~
* ~~Uses a config file that stores at minimum the DB connection string~~
* ~~Integrates the MapQuest API~~
* **Integrates log4j/log4net or similar Log Lib**
* ~~Implements at least 20 Unit Tests~~
  
### Optional
* Write documentation + update wiki after logging has been added
* Export as Json including Image (base64)
* ~~Create Tours~~
* ~~Create, update logs~~
* ~~Navigation~~ -> basics done
* ~~Error Handling / Custom Exceptions~~ -> partly done
* ~~Async API Call~~
* ~~Delete images on Tour delete~~  
* ~~import export~~
* ~~pdf report generation~~
* ~~custom ui component~~ -> done image + caption
* ~~config file~~ -> using App.confing
