
# 📦 EasyMongoNet - A MongoDb Repository for .NET

A generic repository with complete implementations for MongoDB using .NET.

✨ Description

This package provides a complete implementation of a generic repository for .NET applications with MongoDB, simplifying Create, Read, Update, and Delete (CRUD) operations for entities in the database.

With it, you can streamline data access using best practices, abstracting the repository layer and making your application cleaner and more decoupled.

🚀 Installation

You can install the package via NuGet Package Manager or the CLI:

Using NuGet Package Manager:
<pre> Install-Package EasyMongoNet </pre>

🛠️ Configuration

Create a class called MongoDbSettings.cs:

```
appsettings.json:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://userdb:password@localhost:27017/?authMechanism=SCRAM-SHA-256",
    "DatabaseName": "MyStore"
  }
}
```

In your Program.cs:

```csharp
using YourNamespace;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB options
builder.Services.AddEasyMongoNet(builder.Configuration!);

var app = builder.Build();
```
The IDocument interface is designed to standardize the structure of documents stored in MongoDB within a .NET Core application. 

It provides a consistent way to handle common properties for all documents, such as the unique identifier (Id), creation timestamp (CreatedAt), and modification timestamp (ModifiedAt)

```csharp
public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; }

    DateTime CreatedAt { get; set; }

    [BsonIgnoreIfNull]
    DateTime? ModifiedAt { get; set; }
}

public abstract class Document : IDocument
{
    protected Document()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }
    public string Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [BsonIgnoreIfNull]
    public DateTime? ModifiedAt { get; set; }
}
```

🎯 Usage

Creating an Entity

Define an entity in your project:
```csharp
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

Using the Repository

Example of using the generic repository in a Controller:

```csharp
public class ProductsController : ControllerBase
{
    private readonly IMongoRepository<Product> _repository;

    public ProductsController(IMongoRepository<Product> repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        await _repository.InsertOneAsync(product);
        return Ok("Product successfully created!");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products);
    }

    [HttpGet]
    [Route("filter")]
    public async Task<IActionResult> Filter2()
    {
        var product = await _repository.FilterBy(
                    filter => filter.Name.Equals("Smartphone X"),
                    projection => projection.Name
                );

        return Ok(product);
    }
}
```

⚙️ Features

* **AsQueryable()** - Enables LINQ queries on the collection.
* **FilterBy**(Expression<Func<TDocument, bool>> filterExpression) - Filters documents based on a condition.
* **FilterBy**<TProjected>(Expression<Func<TDocument, bool>> filterExpression, Expression<Func<TDocument, TProjected>> projectionExpression) - Filters and projects documents based on conditions and projections.
* **FindOne**(Expression<Func<TDocument, bool>> filterExpression) - Finds a single document matching a condition.
* **FindById**(string id) - Retrieves a document by its ObjectId.
* **FindById**(string field, string id) - Retrieves a document by a specific field.
* **InsertOneAsync**(TDocument document) - Asynchronously inserts a single document.
* **InsertManyAsync**(ICollection<TDocument> documents) - Asynchronously inserts multiple documents.
* **ReplaceOneAsync**(TDocument document) - Replaces an existing document asynchronously.
* **UpdateAsync**(TDocument document) - Updates an existing document asynchronously.
* **UpdateAsync**<TField>(Expression<Func<TDocument, bool>> whereCondition, Expression<Func<TDocument, TField>> field, TField value) - Updates a specific field of a document matching a condition.
* **DeleteOneAsync**(Expression<Func<TDocument, bool>> filterExpression) - Asynchronously deletes a document matching a condition.
* **DeleteManyAsync**(Expression<Func<TDocument, bool>> filterExpression) - Asynchronously deletes multiple documents matching a condition.
* **DeleteByIdAsync**(string id) - Deletes a document by its ObjectId asynchronously.
* **PagedResult<TDocument> GetAllAsync**(int page = 1, int pageSize = 10) - Retrieves all documents paginated.

Performance:

Efficient use of MongoDB database connections.

Generic:

Can be used with any entity class that has an identifier.

🧩 Requirements

* .NET 6+
* MongoDB.Driver 2.29.0+

🗂️ Package Structure

Interfaces:

``` IMongoRepository<T>: Generic repository interface. ```

Implementations:

``` MongoRepository<T>: Concrete implementation. ```

🤝 Contribution

Contributions are welcome!

* Fork the repository.
* Create a branch for your feature (git checkout -b feature/NewFeature).
* Commit your changes (git commit -m "Added a new feature X").
* Push to the branch (git push origin feature/NewFeature).
* Open a Pull Request.

⭐ Give it a Star!

If you find this package useful, don't forget to give it a ⭐ on GitHub!
