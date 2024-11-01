           # 🚗 Car Fleet Management Application

#📖 Overview
The Car Fleet Management application is designed to facilitate the management of vehicle status and fuel entries in a fleet management context.
This application leverages Prism for WPF, enabling modular development, maintainability, and scalability.

#🛠 Technology Choice: Prism for WPF
Why Prism?
Prism is a framework that provides a rich set of features to build robust, maintainable, and testable WPF applications. The key advantages of using Prism include:


#🏗 Application Structure

The application is organized into two main components:

ViewModels:
Responsible for the presentation logic and interaction with the data model.

Models:
Represents the data structures and business logic for managing car status and fuel entries.

#🖥 ViewModels: 
MainViewModel:
Manages the state and behavior of the main view, binding the UI to the data model.

#Data Management:
The MainViewModel utilizes the DataManager to load and manage car status and fuel entry data.

#Observable Collections:
Implements ObservableCollection to enable real-time updates in the UI when data changes.

#📊 Model: 
DataManager
File Management:
The DataManager class is responsible for loading car status and fuel entry data from files specified in the application's configuration.

Data Loading Logic:
Implements methods to read data from the specified files, parse it, and store it in collections.

File System Watching:
Uses FileSystemWatcher to monitor changes in data files and refreshes the data automatically when changes are detected.

#🔄 Logic Flow
Initial Data Loading
When the application starts, the MainViewModel initializes a DataManager instance. The DataManager:

Loads configuration settings to determine the file paths for car status and fuel data.
Reads the data from the specified files and populates the CarStatusCollection and FuelEntryCollection.

Real-time Updates
The application listens for changes to the data files using file watchers. When a change is detected, the following occurs:

- The OnFileChanged method is triggered, which calls ReloadData in the DataManager.
- The DataManager reloads the data, updates the collections, and raises events to notify the MainViewModel.
- The MainViewModel responds to these events and updates the bound collections, ensuring the UI reflects the latest data.

#Data Processing
Data processing involves:

- Aggregating fuel data and updating the car status based on the latest fuel entries.
- Calculating metrics such as total fuel consumed, mileage, and average fuel consumption for each car.

#🏁 Conclusion

The Car Fleet Management application effectively utilizes Prism to create a modular, maintainable, and responsive user interface for managing vehicle and fuel data. 
The architecture supports real-time updates and provides a clear separation of concerns between the UI and business logic.


#How to run this project
- unzip profile into any location on your PC
- Ensure you have dotnet 8 sdk and runtime
- Then run 
