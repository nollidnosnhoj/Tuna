# Why Use Vertical Slice Architecture



When I first started out this project, I initially had my services under the "Entity Service" layer. For instance, methods for getting, creating, updating, and removing audio would go under the "AudioService" class, and then the AudioService class would get registered in the dependency injection container. 

However, when the service class contains a lot of methods, it becomes very hard to find the method you want to update. The file becomes very large, and when working with multiple contributors, you are prone to merge conflicts if multiple people is working on the same service class, but different methods.

So during my free-time of going through Youtube videos and finding things to learn, I stumbled across a video entitled ['Vertical Slice Architecture - Jimmy Bogard'](https://www.youtube.com/watch?v=5kOzZz2vj2o) and it was a presentation by Jimmy Bogard about vertical slicing. What resonated with me about this architecture is how organized it was. Instead of splitting your project by tiers (Data Layer, Business Logic Layer, etc.), you would split your project by features (Audio, Users, etc.). 

Now, when I implement this architecture into my project, you can argue that it is not "fully" vertical slice. The only function that is not part of this vertical slice is the controller, because that belongs into a different project (Audiochan.API) from where the features are located (Audiochan.Core). I could move my features folder into the Audiochan.API project, but what if I want to make another project and want to reference my features. Well, I would have to reference Audiochan.API, which the project's purpose is to create a Web API for web user interfaces. If I want to make a server that handle jobs, I may want to reference the features in my Audiochan.Core folder. So that is why the controllers are separated from the features folder.

So far, I enjoy using this architecture as it makes my files smaller and my features more organized.

