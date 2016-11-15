This repository contains a nuget package called "Automoqer":
https://www.nuget.org/packages/Automoqer/


## About ##

The purpose of Automoqer is to ease the creation of services with constructor IoC in unit testing.

If your services look like this:

```
public class CustomerService 
{
	private readonly ILogger _logger;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IAnotherDependency _anotherDependency;

	public CustomerService(
		ILogger logger,
		IUnitOfWork unitOfWork,
		IAnotherDependency anotherDependency
	
	) 
	{
		_logger = logger;
		_unitOfWork = unitOfWork;
		_anotherDependency = anotherDependency;
	}

	//...
}
```

Then chances are that you have a lot of unit tests that looks like this:

```
[Fact]
public CreateNewCustomerSuccessfully()
{
	var loggerMock = new Mock<ILogger>();
	var unitOfWorkMock = new Mock<IUnitOfWork>();
	var anotherDependencyMock = new Mock<IAnotherDependency>();

	//Your Moq Setup are defined here..

	var service = new CustomerService(
		loggerMock.Object,
		unitOfWorkMock.Object,
		anotherDependencyMock.Object
	);

	//Actual test-case goes here...

	//Your Moq Verify are defined here...
}
```

This is quite tedious to write and if you need to change the service's dependencies, you'll have a lot of test cases to change.

Automoqer removes this boilerplate for you by automatically creates a Service with its constructor parameters as Moq-objects:

```
public CreateNewCustomerSuccessfully()
{
    using (var serviceMocker = new AutoMoqer<CustomerService>())
    {
		//Your Moq Setup are defined here..

		//Actual test-case goes here...

		//Your Moq Verify are defined here...
	}	
}
```

It also runs VerifyAll() on all Moq-objects in it's Dispose-method (hence the IDispose-pattern)