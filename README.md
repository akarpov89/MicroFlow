MicroFlow
=============

[![Build status](https://ci.appveyor.com/api/projects/status/yqymhi8dqekg778u?svg=true)](https://ci.appveyor.com/project/akarpov89/microflow)

Getting Started
----------------

* [What is MicroFlow?](#what-is-microflow)
* [NuGet Package](#nuget-package)
* [Activities](#activities)
* [Nodes](#nodes)
* [Data flow](#data-flow)
* [Flow creation](#flow-creation)
* [Fault handling](#fault-handling)
* [Graph generator](#graph-generator)
* [Sample](#sample)
* [License](https://raw.githubusercontent.com/akarpov89/MicroFlow/master/License.txt)

What is MicroFlow?
---------------------

MicroFlow is a lightweight workflow engine. It allows to build workflows as flowcharts.
Every flow is constructed from a limited number of connected nodes.

Features:
* Code-centric, no XAML
* Data flow friendly: easy to pass data from one activity to another
* Integrated dependency injection
* Flow validation
* Visualization support

Available node types:
* **activity** node represents user-defined action;
* **condition** node represents decision (like `if-else` statement);
* **switch** node represents multiway branch (like `switch` statement);
* **fork-join** node represents concurrent activities;
* **block** node groups several nodes (like blocks in programming languages). 

NuGet Package
---------------

MicroFlow is available on [NuGet](https://www.nuget.org/packages/MicroFlow/) and [MyGet](https://www.myget.org/gallery/microflow/).

Target frameworks:
* .NET 4.0+
* .NET Core
* Portable Libraries (net45+win8+wpa81+wp8)

Activities
-------------

The user-defined activities should inherit from one of the following base classes

##### Activity&lt;TResult&gt;
The most generic activity returning the result of type `TResult`.
An implementation must override the method

```cs
public abstract Task<Result> Execute();
```

##### Activity
The most generic activity without returning value. An implementation must override the method

```cs
protected abstract Task ExecuteCore();
``` 

##### SyncActivity&lt;TResult&gt;
The base class of the synchronous activities returning the value of type `TResult`.
An implemenation must override the method:

```cs
protected abstract TResult ExecuteActivity();
```

##### SyncActivity
The base class of the synchronous activities without returning value.
An implemenation must override the method:

```cs
protected abstract void ExecuteActivity();
```

##### BackgroundActivity&lt;TResult&gt;
Provides the way to execute a function as a separate background task.
An implemenation must override the method:

```cs
protected abstract TResult ExecuteCore();
```

`BackgroundActivity<TResult>` exposes the following properties:
* `CancellationToken` - allows the work to be cancelled;
* `Scheduler` - schedules the worker task;
* `IsLongRunning` - allows to hint `TaskScheduler` that task will be a long-running operation.  

##### BackgroundActivity
Provides the way to execute a function as a separate background task. 
An implemenation must override the method:

```cs
protected abstract void ExecuteCore();
```

`BackgroundActivity` exposes the same properties as `BackgroundActivity<TResult>`.

##### IFaultHandlerActivity
The interface of all fault handlers. Every fault handler must provide the following property:

```cs
Exception Exception { get; set; }
```

Nodes
----------

The `FlowBuilder` class provides the way to create nodes of the flow.

##### ActivityNode&lt;TActivity&gt;

```cs
var node = builder.Activity<SomeActivity>("Node name");

node.ConnectTo(anotherNode)
    .ConnectFaultTo(faultHandler)
    .ConnnectCancellationTo(cancellationHandler);
```

##### ConditionNode

```cs
var node = builder.Condition("Node name");
node.WithCondition(() => boolExpr);

node.ConnectFalseTo(falseBranchNode)
    .ConnectTrueTo(trueBranchNode);
```

There is also an alternative syntax that allows to create `if-then-else` constructs:

```cs
var node = builder
    .If("Condition1", () => boolExpr1).Then(node1)
    .ElseIf("Condition2", () => boolExpr2).Then(node2)
    .Else(node3);    
```

Notice that in this case `node` is initial `ConditionNode` (the one with condition description "Condition description").

##### SwitchNode

```cs
var node = builder.SwitchOf<int>("Node name");
node.WithChoice(() => someIntExpression);

node.ConnectCase(0).To(caseHandler1)
    .ConnectCase(1).To(caseHandler2)
    .ConnectCase(42).To(caseHandler3)
    .ConnectDefault(caseHandler4).
```

##### ForkJoinNode

```cs
var node = builder.ForkJoin("Node name");

var fork1 = node.Fork<SomeForkActivity>("Fork 1 name");
var fork2 = node.Fork<SomeAnotherForkActivity>("Fork 2 name");
var fork3 = node.Fork<SomeActivity>("Fork 3 name");
```

##### BlockNode

```cs
var node = builder.Block("Optional node name", (block, blockBuilder) =>
{
    var activity1 = blockBuilder.Activity<SomeActivity>();
    var activity2 = blockBuilder.Activity<SomeAnotherActivity>();
    
    activity1.ConnectTo(activity2);
});
```

##### Default fault handler

Every activity node should be connected with some specific or default fault handler

```cs
builder.WithDefaultFaultHandler<MyFaultHandler>(); 
```

##### Default cancellation handler

Every activity node should be connected with some specific or default cancellation handler

```cs
builder.WithDefaultCancellationHandler<MyCancellationHandler>();
```

Data flow
--------------

As flow executes data transfers from one activity to another.
The MicroFlow has two mechanisms to define the data flow: _bindings_ and _variables_.

In the examples below we will use the following activities:

```cs
public class ReadIntActivity : SyncActivity<int>
{
    protected override int ExecuteActivity()
    {
        return int.Parse(Console.ReadLine());
    }
}

public class SumActivity : SyncActivity<int>
{
    [Required] public int FirstNumber { get; set; }
    [Required] public int SecondNumber { get; set; }
    
    protected override int ExecuteActivity()
    {
        return FirstNumber + SecondNumber;
    }
}
``` 

##### Binding to activity result

In this example we bind properties `FirstNumber` and `SecondNumber` to the results
of `readFirstNumber` and `readSecondNumber`:

```cs
var readFirstNumber = builder.Activity<ReadIntActivity>();
var readSecondNumber = builder.Activity<ReadIntActivity>();

var sumTwoNumbers = builder.Activity<SumActivity>();

sumTwoNumbers.Bind(a => a.FirstNumber).ToResultOf(readFirstNumber);
sumTwoNumbers.Bind(a => a.SecondNumber).ToResultOf(readSecondNumber);
```

##### Binding to value

In this example we bind `FirstNumber` to the value `42` and `SecondNumber` to `5`:

```cs
var sumTwoNumbers = builder.Activity<SumActivity>();

sumTwoNumbers.Bind(a => a.FirstNumber).To(42);
sumTwoNumbers.Bind(a => a.SecondNumber).To(5);
```

##### Binding to expression

In this example we bind `FirstNumber` to expression using the result of the `readFirstNumber`
and `SecondNumber` to function call `Factorial(5)` expression:

```cs
var readFirstNumber = builder.Activity<ReadIntActivity>();

var firstNumber = Result<int>.Of(readFirstNumber); // Create result thunk

var sumTwoNumbers = builder.Activity<SumActivity>();

sumTwoNumbers.Bind(a => a.FirstNumber).To(() => firstNumber.Get() + 1);
sumTwoNumbers.Bind(a => a.SecondNumber).To(() => Factorial(5)); 
```

##### Using flow variables

The MicroFlow allows to create and use variables scoped to the whole flow or to some specific block:

```cs
var globalVariable = builder.Variable<int>();

var block = builder.Block("my block", (thisBlock, blockBuilder) =>
{
    var localVariable = thisBlock.Variable<string>("initial value");
});
```

It's possible to change the variable value after completion of some activity:

_Assign activity result_:

```cs
 var myVar = builder.Variable<int>();
 
 var readFirstNumber = builder.Activity<ReadIntActivity>();

 myVar.BindToResultOf(readFirstNumber);
```

_Assign some constant value_:

```cs
var myVar = builder.Variable<bool>();
 
var readFirstNumber = builder.Activity<ReadIntActivity>();

readFirstNumber.OnCompletionAssign(myVar, true);
```

_Update value without using activity result_:

```cs
var myVar = builder.Variable<int>(42);
 
var readFirstNumber = builder.Activity<ReadIntActivity>();
 
readFirstNumber.OnCompletionUpdate(
    myVar, 
    oldValue => oldValue + 1
);
```

_Update value using activity result_:

```cs
var myVar = builder.Variable<int>(42);
 
var readFirstNumber = builder.Activity<ReadIntActivity>();
 
readFirstNumber.OnCompletionUpdate(
    myVar, 
    (int oldValue, int result) => oldValue + result
);
```

Later the current value of a variable can be retrieved via property `CurrentValue`:

```cs
var myVar = builder.Variable<int>();
...
var sumTwoNumbers = builder.Activity<SumActivity>();

sumTwoNumbers.Bind(a => a.FirstNumber)
             .To(() => myVar.CurrentValue);
...
```

Flow creation
-----------------

Every flow is a subclass of the `Flow` abstract class. 
The `Flow` base class provides the way to validate and run the
the constructed flow:

```cs
public ValidationResult Validate();
public Task Run();
```

In order to create new flow definition it's required to describe the flow structure 
and give the flow a name:

```cs
public class MyFlow : Flow
{
    public override string Name => "My brand new flow";
    
    protected override void Build(FlowBuilder builder)
    {
        // Create and connect nodes
    }
}
```

The `Flow` сlass also has several configuration extension points:
* `ConfigureServices` method allows to register services required for activities (dependency injection mechanism);
* `ConfigureValidation` method allows to add custom flow validators;
* `CreateFlowExecutionLogger` method allows to setup execution logging.

##### Services registration

Configuring services is possible via overriding the method `ConfigureServices`

```cs
protected virtual void ConfigureServices(
    [NotNull]IServiceCollection services
);
```

Let's say our `ReadIntActivity` uses `IReader` service:

```cs
public interface IReader
{
    string Read();
}

public class ReadIntActivity : SyncActivity<int>
{
    private readonly IReader _reader;
    
    public ReadIntActivity(IReader reader)
    {
        _reader = reader;
    }
    
    protected override int ExecuteActivity() => int.Parse(_reader.Read());
}
```

`ConfigureServices` method allows to register service implementation
passing to the `ReadIntActivity` constructor whenever the activity is created.

`IServiceCollection` has several extension methods for service registration:
* `AddSingleton<TService>(object instance)` registers the specified instance as a service implementation.
* `AddDisposableSingleton<TService>(IDisposable instance)` registers the specified instance as a service implementation;
After finishing the flow execution the instance will be disposed.
* `AddSingleton<TService, TImplementation>()` registers the type of the service implementation. 
The single instance of the `TImplementation` will be used throughout the whole flow;
* `AddSingleton<TService, TImplementation>(params object[] arguments)` the same as previous but allows to specify constructor arguments;
* `AddTransient<TService, TImplementation>()` registers the type of the service implemenation.
The new instance of the `TImplementation` will be created each time it's needed to pass the
service to the activity constructor;
* `AddTransient<TService, TImplementation>(params object[] arguments)` the same as previous but allows to specify constructor arguments.

##### Logging

While no logging is performed by default it's possible to specify the flow execution logger 
by overriding the method:

```cs
protected virtual ILogger CreateFlowExecutionLogger();
```

The `ILogger` interface declares the verbosity level property and several overloads to 
log messages and exceptions.

The MicroFlow provides two simple implementations:
* `NullLogger` does nothing;
* `ConsoleLogger` prints messages to the console.

##### Validation

MicroFlow supports flow validation. Currently by default the following checks are performed:
* Initial node availability;
* Loops absence (node shouldn't point to itself);
* Condition expression presense in condition nodes;
* Choice expression presence in switch nodes;
* Nodes reachability;
* Fault and cancellation handlers availability for activities;
* Selfcontainedness of blocks;
* Acyclicity of blocks;
* Required bindings availablity;
* Activity default constructor availability.

Any `Flow` implementation can add custom validators by overriding the `ConfigureValidation` method:

```cs
protected virtual void ConfigureValidation(
    [NotNull] IValidatorCollection validators
)
```

All validators inherit from the `FlowValidator` abstract class.
`FlowValidator` provides the implementation of visiting every node in the flow and then
performing global validation. Global validation assumes that during the visiting phase validator accumulates 
some information that should be checked later - on the global validation phase. 
`FlowValidator` implementation must override `VisitXxx` methods for each kind of node.
Global validation is fully optional and can be implemented by overriding the `PerformGlobalValidation` method.

Future plans:
* Binding expressions validation;
* Variables scope validation;
* Forks data usage validaton
etc.

Fault handling
----------------------

As it was mentioned earlier if activity ends up with an exception
then fault handler activity takes control.
But there are cases when an exception occures before activity execution. For instance:

* Exception in activity constructor;
* Exception in service constructor;
* Exception during condition expression evaluation;

and so on.

Such situations differ from cases when activity results faulted `Task` and therefore
require special handling.

The `Flow.Run()` method returns a `Task`. 
If the flow completes normally then returned `Task` ends up with the `RanToCompletion` status.
But when an unexpected exception is encountered `Task` completes as `Faulted`.

Thus to avoid unobserved exceptions one should attach a continuation to the `Task` returned from
the `Run` method:

```cs
var flow = new MyFlow();

var flowTask = flow.Run();

flowTask.ContinueWith(t =>
{
    if (t.IsFaulted)
    {
        // Handle exception
    }
    else if (t.Status == TaskStatus.RanToCompletion)
    {
        // Flow is completed normally
    }
});
```

Graph generator
--------------------

The MicroFlow comes with the tool called _MicroFlow.Graph_ that allows to generate *.dgml files.
DGML is an XML-based file format for directed graphs supported by the Microsoft Visual Studio 2010 and later.

MicroFlow.Graph.exe is a console program with two required arguments:
* path to or name of the assembly containing the flow definition class;
* flow class name.

Example: `MicroFlow.Graph MicroFlow.Test.dll Flow1`

The generated sample flow is presented below.

> **Note:** Graph generation is available only if the flow has a default constructor

Sample
-------------

Let's create the simple flow: 
read two numbers and if first number greater than a second output "first > second" 
otherwise output "first <= second". The graphical scheme of the flow is presented below. 
![ExampleFlow1](https://raw.github.com/akarpov89/MicroFlow/master/content/flow1.png)

At first let's create activity for reading numbers. It will use the following `IReader` interface.

```cs
public interface IReader
{
    string Read();
}
```

Because reading activity is synchronous and returns an integer 
it will inherit from the `SyncActivity<int>` class.

```cs
public class ReadIntActivity : SyncActivity<int>
{
    private readonly IReader _reader;

    public ReadIntActivity(IReader reader)
    {
        _reader = reader;
    }

    protected override int ExecuteActivity() => int.Parse(_reader.Read());
}
```

Now let's create output activity. It will use the following `IWriter` interface:

```cs
public interface IWriter
{
    void Write(string message);
}
```

Because output activity is synchronous and doesn't return any value
it will inherit from the `SyncActivity` class. Also output activity requires a message to print out.
This can be experessed by declaring the property marked with `[Required]` attribute.

```cs
public class WriteMessageActivity : SyncActivity
{
    private readonly IWriter _writer;

    public WriteMessageActivity(IWriter writer)
    {
        _writer = writer;
    }

    [Required]
    public string Message { get; set; }

    protected override void ExecuteActivity() 
    {
        _writer.Write(Message);
    }
}
```

Every activity may fail or be cancelled. That's why we also need to define fault handler 
and cancellation handler activities:

```cs
public class MyFaultHandler : SyncActivity, IFaultHandlerActivity
{
    public Exception Exception { get; set; }
    
    protected override void ExecuteActivity()
    {
        Console.WriteLine(Exception);
    }
}

public class MyCancellationHandler : SyncActivity
{
    protected override void ExecuteActivity()
    {
        Console.WriteLine("Cancelled");
    }
}
```

Before creating the flow itself we should provide the implementations of the `IReader` and `IWriter`
services:

```cs
public class ConsoleReader : IReader
{
    public string Read() => Console.ReadLine();
}

public class ConsoleWriter : IWriter
{
    public void Write(string message)
    {
        Console.WriteLine(message);
    }
}
```

Now we are ready to define the flow. All flows inherit from the `Flow` class. 
This base class allows to build the flow structure, configure the dependency injection and run the flow.

```cs
public class Flow1 : Flow
{
    public override string Name => "Flow1. Uses condition node";

    protected override void Build(FlowBuilder builder)
    {
        // Create reading activity nodes.
        var inputFirst = builder.Activity<ReadIntActivity>();
        inputFirst.WithName("Read first number");
        
        var inputSecond = builder.Activity<ReadIntActivity>();
        inputSecond.WithName("Read second number");

        // Create bindings to the results.
        var first = Result<int>.Of(inputFirst);
        var second = Result<int>.Of(inputSecond);

        // Create condition node.
        var condition = builder.Condition();
        condition.WithName("If first number > second number");
        
        // Set condition to the expression.
        condition.WithCondition(() => first.Get() > second.Get());

        // Create true branch output activity.
        var outputWhenTrue = builder.Activity<WriteMessageActivity>();
        outputWhenTrue.WithName("Output: first > second");
        
        // Bind the output message to the expression.
        outputWhenTrue.Bind(x => x.Message)
                      .To(() => $"{first.Get()} > {second.Get()}");

        // Create false branch output activity.
        var outputWhenFalse = builder.Activity<WriteMessageActivity>();
        outputWhenFalse.WithName("Output: first <= second");
        
        // Bind the output message to the expression.
        outputWhenFalse.Bind(x => x.Message)
                       .To(() => $"{first.Get()} <= {second.Get()}");

        // Set initial node of the flow.
        builder.WithInitialNode(inputFirst);
        
        // Set default fault and cancellation handlers.
        builder.WithDefaultFaultHandler<MyFaultHandler>();
        builder.WithDefaultCancellationHandler<MyCancellationHandler>();

        //
        // Connect nodes.
        //

        inputFirst.ConnectTo(inputSecond);
        inputSecond.ConnectTo(condition);

        condition.ConnectTrueTo(outputWhenTrue)
                 .ConnectFalseTo(outputWhenFalse);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // Register services.
        services.AddSingleton<IReader, ConsoleReader>();
        services.AddSingleton<IWriter, ConsoleWriter>();
    }
}
```

That's it. Now we can create the instance of the flow and run it:

```cs
public static void Main(string[] args)
{
    var flow = new Flow1();
    flow.Run();
}
```


### Special thanks

<img src="https://pbs.twimg.com/profile_images/674917637646716928/lni0by_I.png" width="64px" />

[ReSharper](http://www.jetbrains.com/resharper/) - the most advanced productivity add-in for Visual Studio!
