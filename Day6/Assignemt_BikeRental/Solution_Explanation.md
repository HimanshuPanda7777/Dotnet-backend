# Detailed Beginner's Guide to the Bike Rental Solution

Hello! Since you're still learning .NET, I have broken down every single part of this solution. By reading this, you will understand **what** the code does, **why** it does it, and exactly **how** to explain it to your trainer like a pro.

Once you are done, you can delete this file!

---

## 1. Why Three Different Files?
Instead of dumping all the code into one big file, we split it into three: `Bike.cs`, `BikeUtility.cs`, and `Program.cs`. 
**What to tell your trainer:** "I separated the code into three files to follow the **Single Responsibility Principle**. It keeps the code clean—one file holds the data blueprint, one handles the logic, and one handles the user interface (the console)."

---

## 2. Breaking Down `Bike.cs`

```csharp
public class Bike
{
    public string Model { get; set; }
    public int PricePerDay { get; set; }
    public string Brand { get; set; }
}
```
**What is a Class?** 
A class is like a blueprint. Just like a blueprint for a house tells you where the doors are, the `Bike` class tells C# what information every single bike MUST have (a Model, a Price, and a Brand).

**What are `{ get; set; }`?** 
These are called **Properties**. 
- `get` allows us to *read* the value (e.g., finding out the bike's brand).
- `set` allows us to *write* the value (e.g., assigning the brand as "Honda").
Using properties instead of standard variables is a C# best practice because it protects the data (called **Encapsulation**).

---

## 3. Breaking Down `BikeUtility.cs`

This file is where we put our "Action" or "Logic" methods.

### Method 1: `AddBikeDetails`
```csharp
public void AddBikeDetails(string model, string brand, int pricePerDay)
{
    Bike newBike = new Bike
    {
        Model = model,
        Brand = brand,
        PricePerDay = pricePerDay
    };

    int newKey = Program.bikeDetails.Count + 1;
    Program.bikeDetails.Add(newKey, newBike);
}
```
**Step-by-step explanation:**
1. **`Bike newBike = new Bike { ... }`**: We are creating a brand-new object (an actual, physical instance of our blueprint) using the details the user typed in.
2. **`Program.bikeDetails.Count`**: The dictionary holds all our bikes. `.Count` tells us how many bikes are currently in it.
3. **`+ 1`**: If there are 0 bikes, `0 + 1 = 1`. The first bike gets ID 1. The second bike gets ID 2. This satisfies the requirement: *"The key of the dictionary should be one more than the current number of items"*.
4. **`.Add(newKey, newBike)`**: We insert the unique ID number and the bike object together into our global dictionary.

### Method 2: `GroupBikesByBrand`
```csharp
public SortedDictionary<string, List<Bike>> GroupBikesByBrand()
```
**Wait, what is a `SortedDictionary<string, List<Bike>>`?**
- A **Dictionary** is a collection that stores "Key-Value" pairs. Think of it like a real dictionary: The Key is the word, the Value is the definition.
- Here, our Key is the **Brand** (a `string`, like "Honda").
- Our Value is a **List of Bikes** (`List<Bike>`). Why a List? Because Honda makes more than one bike! We need a list to hold all the Hondas.
- **SortedDictionary** means it will automatically sort the keys alphabetically (e.g., "Honda" will automatically show up before "Kawasaki").

**How the grouping loop works:**
```csharp
foreach (var item in Program.bikeDetails)
```
- We loop through every single bike we have saved so far.
```csharp
if (groupedResult.ContainsKey(currentBike.Brand))
{
    groupedResult[currentBike.Brand].Add(currentBike);
}
```
- **`ContainsKey`** checks: *Do we already have a list for "Honda" in our new grouped dictionary?*
- If **Yes**: We just grab that existing "Honda" list and `.Add()` our current bike to it.
```csharp
else
{
    List<Bike> newBikeList = new List<Bike>();
    newBikeList.Add(currentBike);
    groupedResult.Add(currentBike.Brand, newBikeList);
}
```
- If **No**: This is the first time we are seeing a "Honda"! So we create a brand new, empty `List<Bike>`, add our current bike to it, and then put this brand new list into the dictionary under the name "Honda".

---

## 4. Breaking Down `Program.cs`

This is where the program actually starts running (`Main` method) and talks to the user.

### The Global Dictionary
```csharp
public static SortedDictionary<int, Bike> bikeDetails = new SortedDictionary<int, Bike>();
```
**Why `public static`?**
- `static` means this dictionary belongs to the entire program itself, not a specific object. Because it is static, we can access it directly from our `BikeUtility` class by simply typing `Program.bikeDetails`. 

### The `do-while` Loop
```csharp
do
{
    // Print Menu
} while (choice != 3);
```
**Why did we use a `do-while` loop?**
A `do-while` loop is perfect for Console Menus because it guarantees that the code inside the loop will run **at least once** before it checks the condition at the end. We always want to show the menu to the user first!

### Getting the User's Choice
```csharp
string input = Console.ReadLine();
if (!int.TryParse(input, out choice))
{
    continue;
}
```
**`int.TryParse` vs `int.Parse`:**
- Later on, for the price, we used `int.Parse()`. `Parse` takes a string and forces it to become an integer. If the user types "Hello", `int.Parse` will crash the program.
- For the main menu choice, we used `int.TryParse`. This is safer! It says: *"Try to turn this text into a number. If it works, save it in the `choice` variable. If it fails (like if they typed a letter), just ignore it and restart the loop (`continue;`)."*
- **What to tell your trainer:** "I used `TryParse` for the menu choice to prevent the application from crashing if the user accidentally hits a letter key."

### Displaying the Grouped Bikes
```csharp
foreach (var brandGroup in groupedBikes)
{
    foreach (var bike in brandGroup.Value)
    {
        Console.WriteLine($" {bike.Brand} {bike.Model}");
    }
}
```
**Why two loops? (Nested Loops)**
- The first loop (`foreach var brandGroup`) goes through the Brands (e.g., First loop it grabs "Honda" and its list of bikes).
- The second loop (`foreach var bike`) digs into that specific list and prints out each bike one by one. The `.Value` part refers to the `List<Bike>`.

---

## Summary for your review:
If your trainer asks you questions, keep these key phrases in mind:
1. **"I used a SortedDictionary to automatically organize the brands alphabetically."**
2. **"I mapped the Brand to a `List<Bike>` because one brand can have multiple different models."**
3. **"I put the logic in `BikeUtility.cs` to separate it from the Console UI, which keeps the code clean and maintainable."**

You've got this! Delete this file when you're ready!
