# Log

## Asummptions
So, we have two products, and I need to implement a method that will select them and send the result to the message bus.
In addition to selection and actual processing, we need to validate the user and emit relevant events to the bus throughout the processing flow.

## Decisions
I'll encapsulate the service invocation logic inside adapters and create a separate class that maps ProductCode to the corresponding adapter.
This class will expose two segregated interfaces:
* One for registration (which will happen during singleton registration or initialization of specific IAdministrationServices, etc.), and
* Another one for usage.

There’s also some validation logic that applies to both products. To comply with the Single Responsibility Principle, this logic will be implemented separately.
Since the number of validation rules may grow over time, I'll use the FluentValidation framework. Some rules may depend on parameters, so we’ll have parameterized validations.

Overall, it seems that for each product, we need to specify:
A set of validation rules
The adapter implementation

Since validation settings might change and appear portable across products, I'll wrap validation logic in a ValidationFactory. This factory will allow creation of validation rules from a set of POCOs (ValidationParameter), making it possible to serialize the rules and store them in configuration, a database, or a config/discovery service.

For example, the config might look like this:

"Validators": [
  {
    "MinAge": 18,
    "MaxAge": 39
  },
  {
    "MinimumPayment": 0.99
  }
]

These parameters will automatically be converted into validation rules for Product 1.

## Observations
If we assume there’s a DI container set up at the start, it would be cleaner to keep validation logic inside each adapter and let the container scan and register them. However, since registration and instantiation strategies are not yet defined, I'll keep a Configurator as an explicit entry point for now.

For the second service, any errors will be wrapped into exceptions. I’ve decided not to introduce any global exception wrappers for now, as I don’t yet know how these exceptions are processed down the line. Usually, such wrappers are handled at the entry point — e.g., request pipeline, job runner, etc.

## Todo
Maybe it makes sense to think about mapper for the transformations, but looks like it's one time excersize and I don't see any scenario when it would make sense to reuse mappings.
Also, validators can be applied to the Application class with attributes. Maybe it makes sense to play with it.