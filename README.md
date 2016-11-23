This repository contains a nuget package called "Automoqer": https://www.nuget.org/packages/Automoqer/


# About Automoqer #

The purpose of Automoqer is to ease the creation of services with constructor IoC in unit testing.


## How to use ##

1. Get Automoqer via NuGet: [![NuGet](https://img.shields.io/nuget/v/tusdotnet.svg)](https://www.nuget.org/packages/Automoqer/)

2. In your unit test, create the Automoqer like this:

```csharp
using (var serviceMocker = new AutoMoqer<ServiceToCreate>())
{	
	//Example definition of a dependency mock setup:
	serviceMocker.Param<ICustomerRepository>().Setup(m => m.FindCustomer(It.Is<int>(p => p == 1))).Returns(new Customer());

	//Access the service instance:
	var service = serviceMocker.Service;

	//Example verification of a method call
	serviceMocker.Param<ILogger>().Verify(m => m.Log(It.IsAny<string>));
}
```

## Introduction ##

If your services are defined like this:

```csharp
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

```csharp
[Fact]
public CreateNewCustomerSuccessfully()
{
	var loggerMock = new Mock<ILogger>();
	var unitOfWorkMock = new Mock<IUnitOfWork>();
	var anotherDependencyMock = new Mock<IAnotherDependency>();

	//Your Moq .Setup are defined here..

	var service = new CustomerService(
		loggerMock.Object,
		unitOfWorkMock.Object,
		anotherDependencyMock.Object
	);

	//Actual test-case goes here...

	//Your Moq .Verify are defined here...
}
```

This is quite tedious to write and if you need to change the service's dependencies, you'll have a lot of test cases to change.

Automoqer removes this boilerplate for you by automatically create a Service with its constructor parameters as Moq-objects:

```csharp
[Fact]
public CreateNewCustomerSuccessfully()
{
    using (var serviceMocker = new AutoMoqer<CustomerService>())
    {
		//Your Moq .Setup are defined here..
		//Mocks accessed by serviceMocker.Param<ILogger>().Setup(...

		//Actual test-case goes here...
		//Service accessed by serviceMocker.Service

		//Your Moq .Verify are defined here...
	}	
}
```

It also runs VerifyAll() on all Moq-objects in its Dispose-method (hence the IDisposable-pattern)



# Contributors #

 * Robert Bengtsson - https://github.com/rbengtsson
 * Stefan Matsson - https://github.com/smatsson


# License and usage

MIT License

Copyright (c) 2016 Robert Bengtsson

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

