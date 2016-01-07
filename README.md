# MicroFlow

[![Build status](https://ci.appveyor.com/api/projects/status/yqymhi8dqekg778u?svg=true)](https://ci.appveyor.com/project/akarpov89/microflow)

MicroFlow is a lightweight workflow engine. It allows to build workflows as flowcharts.
Every flow is constructed from a limited number of connected nodes. There are several node types:
* **activity** node represents user-defined action;
* **condition** node represents decision (like `if-else` statement);
* **switch** node represents multiway branch (like `switch` statement);
* **fork-join** node represents concurrent activities;
* **block** node groups several nodes (like blocks in programming languages).
 
Because user-defined action may fail or be cancelled activity nodes can be connected not only 
to the next operation node but also to the _fault handler_ node and _cancellation handler_ node.

#### Example


#### Special thanks

![ReSharper](http://www.jetbrains.com/img/logos/logo_resharper_small.gif)  
[ReSharper](http://www.jetbrains.com/resharper/) - the most advanced productivity add-in for Visual Studio!
