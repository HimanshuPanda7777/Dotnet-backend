using System;

double price;
while (true)
{
    Console.Write("Price: ");
    if (double.TryParse(Console.ReadLine(), out price) && price >= 0)
    {
        break;
    }
    Console.WriteLine("Error: Please enter a valid positive number for price.");
}

int quantity;
while (true)
{
    Console.Write("Quantity: ");
    if (int.TryParse(Console.ReadLine(), out quantity) && quantity >= 0)
    {
        break;
    }
    Console.WriteLine("Error: Please enter a valid positive whole number for quantity.");
}

double discount;
while (true)
{
    Console.Write("Discount: ");
    if (double.TryParse(Console.ReadLine(), out discount) && discount >= 0 && discount <= 100)
    {
        break;
    }
    Console.WriteLine("Error: Please enter a valid discount percentage between 0 and 100.");
}

double subtotal = price * quantity;
double discountAmount = subtotal * (discount / 100);
double finalAmount = subtotal - discountAmount;

Console.WriteLine("\n--- Receipt ---");
Console.WriteLine($"Subtotal: ${Math.Round(subtotal, 2)}");
Console.WriteLine($"Discount amount: ${Math.Round(discountAmount, 2)}");
Console.WriteLine($"Final payable amount: ${Math.Round(finalAmount, 2)}");
