using System;
using System.Collections.Generic;
using System.Linq;

namespace Customer
{
    interface IPromotionalProgram {
        double calculateDiscount(Order order);
    }
    class EveryFifthProduct : IPromotionalProgram {
        public double calculateDiscount (Order order) {
            if(order.getCount() >= 5) {
                int times = (int)order.getCount() / 5;
                double discount = 0;
                for (int i = 0; i < times; ++i) {
                    discount += order[(i + 1) * 5].Commodity.Price / 2;
                }
                return discount;
            }
            return 0;
        }
    }
    class TotalSumDiscount : IPromotionalProgram {
        public double calculateDiscount (Order order) {
            if (order.getTotalSum() >= 500) {
                return order.getTotalSum() * 0.2;
            }
            return 0;
        }
    }
    class ExpensiveProductDiscount : IPromotionalProgram {
        public double calculateDiscount (Order order) {
            double discount = 0;
            for (int i = 0; i < order.getCount(); ++i) {
                if (order[i].Commodity.Price >= 700) {
                    discount += (order[i].Commodity.Price * 0.4) * order[i].Amount;
                }
            }
            return discount;
        }
    }
    class Product {
        private string name;
        private double price;

        public Product () {
            this.name = "Bread";
            this.price = 1;
        }
        public Product (string name, double price) {
            this.name = name;
            this.price = price;
        }

		public override string ToString()
		{
            return "  " + this.name + $"{this.price:C}".PadLeft(25 - this.name.Length - 2);
		}

		public string Name { get { return this.name; } private set { this.name = value; }}
        public double Price { get { return this.price; } private set { this.price = value; }}
    }
    class OrderLine {
        private Product commodity;
        private int amount;

        public OrderLine () {
            this.commodity = new Product();
            this.amount = 1;
        }
        public OrderLine (Product commodity, int amount) {
            this.commodity = commodity;
            this.amount = amount;
        }

		public override string ToString() {
            return this.commodity.ToString() + $"\n    x{this.amount}" ;
		}

		public Product Commodity { get { return this.commodity; } private set { this.commodity = value; }}
        public int Amount { get { return this.amount; } private set { this.amount = value; }}
    }
    class Order {
        private List<OrderLine> orderLines;
        private IPromotionalProgram discount;
        private bool endOfPurchase = false;

        public void setDiscount (IPromotionalProgram promotionalProgram) {
            this.discount = promotionalProgram;
        }
        public double calculateMyDiscount () {
            if (this.discount != null) {
                return discount.calculateDiscount(this);
            }
            return 0;
        }
        public int getCount () {
            return this.orderLines.Count;
        }
        public double getTotalSum () {
            double sum = 0;
            foreach (OrderLine elem in this.orderLines) {
                sum += (elem.Commodity.Price)*elem.Amount;
            }
            return sum;
        }

        public Order () {
            this.orderLines = new List<OrderLine>();
            this.discount = null;
        }
        public Order (OrderLine orderLine) {
            this.orderLines = new List<OrderLine>();
            this.orderLines.Add(orderLine);
            this.discount = null;
        }
        public Order (List<OrderLine> orderLines) {
            this.orderLines = new List<OrderLine>();
            foreach (OrderLine elem in orderLines) {
                this.orderLines.Add(elem);
            }
            this.discount = null;
        }

        public void buy (Product product, int amount) {
            if (endOfPurchase == false) {
                this.orderLines.Add(new OrderLine(product, amount));
            }
            else {
                Console.WriteLine("The check is already printed. Please, start a new purchase.");
            }
        }
        public void printCheck () {
            this.endOfPurchase = true;
            string divider = new string('-', 25);
            Console.WriteLine($"         > CHECK\n  Сashier: Сashier1\n{divider}");
            foreach (OrderLine elem in orderLines) {
                Console.WriteLine(elem);
            }
            Console.WriteLine($"{divider}\n" + $"Total price: {this.getTotalSum():C}".PadLeft(25));
            Console.WriteLine($"Discount: {this.calculateMyDiscount():C}".PadLeft(25));
            Console.WriteLine($"{divider}\n" + $"PRICE TO PAY: {(this.getTotalSum() - this.calculateMyDiscount()):C}".PadLeft(25) + '\n');
        }

		public OrderLine this[int index] {
            get { return this.orderLines[index]; }
        }
    }
    class Customer {
        private List<Order> orders;
        private List<IPromotionalProgram> promotionalPrograms;

        private int IndexOfMaxDiscountForOrder () {
            if (this.promotionalPrograms != null) {
                int index = 0;
                for (int i = 1; i < this.promotionalPrograms.Count; ++i) {
                    if (this.promotionalPrograms[i].calculateDiscount(this.orders.Last()) > this.promotionalPrograms[index].calculateDiscount(this.orders.Last())) {
                        index = i;
                    }
                }
                return index;
            }
            return -1;
        }
        private void setDiscountForOrder () {
            if (this.promotionalPrograms != null) {
                int index = this.IndexOfMaxDiscountForOrder();
                if (index != -1) {
                    this.orders.Last().setDiscount(this.promotionalPrograms[this.IndexOfMaxDiscountForOrder()]);
                }
            }
        }

        public Customer () {
            this.orders = new List<Order>();
            this.promotionalPrograms = new List<IPromotionalProgram>();
        }
        public Customer (IPromotionalProgram promotionalProgram) {
            this.promotionalPrograms = new List<IPromotionalProgram>();
            this.promotionalPrograms.Add(promotionalProgram);
        }
        public Customer (List<IPromotionalProgram> promotionalPrograms) {
            this.promotionalPrograms = promotionalPrograms;
        }
        public Customer (Order order, List<IPromotionalProgram> promotionalPrograms = null) {
            this.orders = new List<Order>();
            this.orders.Add(order);
            this.promotionalPrograms = promotionalPrograms;
        }

        public void startOrder () {
            Order order = new Order();
            this.orders.Add(order);
        }
        public void buy (Product product, int amount) {
            this.orders.Last().buy(product, amount);
        }
        public void commitOrder () {
            this.setDiscountForOrder();
            this.orders.Last().printCheck();
        }

        public void printOrders () {
            if (this.orders.Count > 0) {
                foreach (Order elem in this.orders) {
                    elem.printCheck();
                }
            }
        }
        public void printPromotionalPrograms () {
            if (this.promotionalPrograms.Count > 0) {
                Console.WriteLine("A customer signed for such discount(s):");
                foreach (IPromotionalProgram elem in this.promotionalPrograms) {
                    Console.WriteLine(elem.GetType());
                }
            }
        }
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            Product a = new Product();
            Product b = new Product("Ice-cream", 0.5);
            OrderLine c = new OrderLine(a, 2);
            OrderLine d = new OrderLine(b, 3);
            List<OrderLine> q = new List<OrderLine>();
            q.Add(c); q.Add(d);
            Order w = new Order(q);

            List<IPromotionalProgram> listP = new List<IPromotionalProgram>();
            EveryFifthProduct discount1 = new EveryFifthProduct();
            TotalSumDiscount discount2 = new TotalSumDiscount();
            ExpensiveProductDiscount discount3 = new ExpensiveProductDiscount();
            listP.Add(discount1);
            listP.Add(discount2);
            listP.Add(discount3);


            Customer p = new Customer(w, listP);
            p.startOrder();
            p.buy(new Product("Nesquik", 2), 3);
            p.buy(new Product("Milk", 1), 1);
            p.buy(new Product("Cheese", 0.20), 1);
            p.buy(new Product("Bread", 1), 1);
            p.buy(new Product("Butter", 1.5), 1);
            p.buy(new Product("Yogurt", 0.4), 6);
            p.commitOrder();


            //Console.WriteLine("{0:C}", 10);
        }
    }
}
