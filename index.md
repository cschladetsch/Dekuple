
# Liminal **Dekuple** Documentation.
A dependancy-injection and entity system designed from the ground up to support both rapid prototyping and long-term development and support of `Unity3d` based applications.

The system also adds a `MVC` or `Model-View-Controller` pattern.

Except in this case, the `Controller` is called an `Agent` and uses the `Flow` library, and `Views` are based on `MonoBehavior`s.

The basic architecture is a morphism of a number ideas combined together into an integrated whole:

- Dependancy Injection
- Object Registry/Factory for persistence and networking
- Model/View/Controller, or Model/ViewController/View
- Unity3d Prefabs and Behaviours
- Reactive programming techniques

The code is not 'tricky', other than the hoops required for templates in C#. The key hurdle a user will face is simply understanding the architecture and semantics of the seemingly simple systems.

One key understanding required is that one Registry<T> class is used for each model/agent/view domains, but of course with a different template parameter T.

There are Readme's in each substantial sub-folder that describes each component in more detail.

