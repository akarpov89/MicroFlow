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

### Activities

The user-defined activities should inherit from one of the following base classes

##### 1. Activity&lt;TResult>&gt;
The most generic activity returning the result of type `TResult`.
An implementation must override the method
```cs
public abstract Task<Result> Execute();
```

##### 2. Activity
The most generic activity without returning value. An implementation must override the method
```cs
protected abstract Task ExecuteCore();
``` 

##### 3. SyncActivity&lt;TResult>&gt;
The base class of the synchronous activities returning the value of type `TResult`.
An implemenation must override the method:
```cs
protected abstract TResult ExecuteActivity();
```

##### 4. SyncActivity
The base class of the synchronous activities without returning value.
An implemenation must override the method:
```cs
protected abstract void ExecuteActivity();
```

##### 5. BackgroundActivity&lt;TResult>&gt;
Provides the way to execute a function on a separate thread.
An implemenation must override the method:
```cs
protected abstract TResult ExecuteCore(CancellationToken token);
```

##### 6. BackgroundActivity
Provides the way to execute an action on a separate thread. 
An implemenation must override the method:
```cs
protected abstract void ExecuteCore(CancellationToken token);
```

##### 7. IFaultHandlerActivity
The interface of all fault handlers. Every fault handler must provide the following property:
```cs
Exception Exception { get; set; }
```

### Nodes

The `FlowBuilder` class provides the way to create nodes of the flow.

##### 1. ActivityNode&lt;TActivity&gt;
```cs
var node = builder.Activity<SomeActivity>("Optional node name");

node.ConnectTo(anotherNode)
    .ConnectFaultTo(faultHandler)
    .ConnnectCancellationTo(cancellationHandler);
```

##### 2. ConditionNode
```cs
var node = builder.Condition("Optional node name");
node.WithCondition(() => someBooleanExpression);

node.ConnectFalseTo(falseBranchNode)
    .ConnectTrueTo(trueBranchNode);
```

##### 3. SwitchNode
```cs
var node = builder.SwitchOf<int>("Optional node name");
node.WithChoice(() => someIntExpression);

node.ConnectCase(0).To(caseHandler1)
    .ConnectCase(1).To(caseHandler2)
    .ConnectCase(42).To(caseHandler3)
    .ConnectDefault(caseHandler4).
```

##### 4. ForkJoinNode
```cs
var node = builder.ForkJoin("Optional node name");

var fork1 = node.Fork<SomeForkActivity>("Optional fork name");
var fork2 = node.Fork<SomeAnotherForkActivity>("Optional fork name");
var fork3 = node.Fork<SomeActivity>("Optional fork name");
```

##### 5. BlockNode
```cs
var node = builder.Block("Optional node name", (block, blockBuilder) =>
{
    var activity1 = blockBuilder.Activity<SomeActivity>();
    var activity2 = blockBuilder.Activity<SomeAnotherActivity>();
    
    activity1.ConnectTo(activity2);
});
```

### Example

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

    protected override void ExecuteActivity() => _writer.Write(Message);
}
```

Every activity may fail or be cancelled. That's why we also need to define fault handler 
and cancellation handler activities:

```cs
public class MyFaultHandler : SyncActivity, IFaultHandlerActivity
{
    public Exception Exception { get; set; }
    
    protected override void ExecuteActivity() => Console.WriteLine(Exception);
}

public class MyCancellationHandler : SyncActivity
{
    protected override void ExecuteActivity() => Console.WriteLine("Cancelled");
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
    public string Write(string message) => Console.WriteLine(message);
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
        var inputFirst = builder.Activity<ReadIntActivity>("Read first number");
        var inputSecond = builder.Activity<ReadIntActivity>("Read second number");

        // Create bindings to the results.
        var first = Result<int>.Of(inputFirst);
        var second = Result<int>.Of(inputSecond);

        // Create condition node.
        var condition = builder.Condition("If first number > second number");
        
        // Set condition to the expression.
        condition.WithCondition(() => first.Get() > second.Get());

        // Create true branch output activity.
        var outputWhenTrue = builder.Activity<WriteMessageActivity>("Output: first > second");
        
        // Bind the output message to the expression.
        outputWhenTrue.Bind(x => x.Message).To(() => $"{first.Get()} > {second.Get()}");

        // Create false branch output activity.
        var outputWhenFalse = builder.Activity<WriteMessageActivity>("Output: first <= second");
        
        // Bind the output message to the expression.
        outputWhenFalse.Bind(x => x.Message).To(() => $"{first.Get()} <= {second.Get()}");
            
        // Create fault & cancellation handler nodes.
        var faultHandler = builder.FaultHandler<MyFaultHandler>("Global fault handler");
        var cancellationHandler = builder.Activity<MyCancellationHandler>("Global cancellation handler");

        // Set initial node of the flow.
        builder.WithInitialNode(inputFirst);
        
        // Set default fault and cancellation handlers.
        builder.WithDefaultFaultHandler(faultHandler);
        builder.WithDefaultCancellationHandler(cancellationHandler);

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

![ReSharper](http://www.jetbrains.com/img/logos/logo_resharper_small.gif)  
[ReSharper](http://www.jetbrains.com/resharper/) - the most advanced productivity add-in for Visual Studio!
