# OpenWeatherMap_API

- How much time did you spend on this task? 1.5h.
  
- If you had more time, what improvements or additions would you make? I would write more detailed unit tests for it.
  
- What is the most useful feature recently added to your favorite programming language? My favorite programming language is Golang and the new added feature is "Type Aliases for Error Handling".

- Please include a code snippet to demonstrate how you use it:
    type MyError = error

    func someFunction() MyError {
      return fmt.Errorf("something went wrong!!!!")
    }

- How do you identify and diagnose a performance issue in a production environment? I can think of several steps: 1. Understand symptoms and signs as to where the issue might be coming from and all the parties that are affected by the issue, 2. Try to reproduce the same exact problem in a staging or non-production environment. 3. Inspect logs and error rates, 4. monitor and watch out for database perfomance, 5. analyze code perfomance.
- Have you done this before? We were noticing a perfomance issue when developing a cash tranfer system, where the transfer was too slow. So we decided to move parts of the business from the code to the Stored Procedures in DB2 and reduced reduntant calls to the server in doing so. 

- What's the last technical book you read or technical conference you attended? I'm halfway through "Hands-On Machine Learning with Scikit-Learn, Keras, and TensorFlow".
- What did you learn from it? I learned about different types of machine learning algorithms, from the preprocessing stage up to deploying them using pipelines and also did project using the knowledge.

- What's your opinion about this technical test? It was a good technical test assessing API designing skills and communicative ones.

- Please describe yourself using JSON format.
{
  "name": "Kasra Nadri",
  "appearance": {
    "hairColor": "black",
    "eyeColor": "dark brown",
    "height": "181cm",
    "style": "casual"
  },
  "interests": ["video making", "software development", "running", "public speaking", "teaching"],
  "favoriteShows": ["True Detective(Season 1&2)", "The Bear", "Breaking Bad", "Brooklyn Nine-Nine"]
}
  
  
