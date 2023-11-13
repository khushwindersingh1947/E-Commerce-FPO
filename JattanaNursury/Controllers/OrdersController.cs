﻿using AutoMapper;
using JattanaNursury.Data;
using JattanaNursury.Models;
using JattanaNursury.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;

namespace JattanaNursury.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public class ProductModel
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }

        [HttpGet]
        public async Task<List<ProductModel>> GetProductsByNameAsync(string search)
        {
            List<ProductModel> products = new();
            if (string.IsNullOrEmpty(search)) return products;

            try
            {
                var pList = await _context.Products.Where(a => a.Name.ToLower().Contains(search.ToLower())).ToListAsync();
                var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductModel>()));
                products = mapper.Map<List<Product>, List<ProductModel>>(pList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return products;
        }

        [HttpPost]
        public async Task<IActionResult> SaveSaleOrderAsync([FromBody] OrderViewModel saleOrder)
        {
            if (saleOrder == null) return View(nameof(Index));

            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<OrderViewModel, Customer>()));
            var customer = mapper.Map<OrderViewModel, Customer>(saleOrder);
            _context.Customers.Add(customer);

            var order = new Order { CustomerId = customer.Id, OrderDate = DateTime.UtcNow, Discount = saleOrder.Discount, EmployeeId = saleOrder.Employee };

            decimal totalPrice = 0;
            foreach (var item in saleOrder.Products)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);


                if (product == null) continue;
                if (product.Quantity < item.Quantity)
                {
                    return RedirectToAction(nameof(Index));
                }
                var productOrder = new ProductOrder { ProductId = product.Id, Quantity = item.Quantity, TotalPrice = product.UnitPrice * item.Quantity };
                totalPrice += productOrder.TotalPrice;
                order.ProductOrders?.Add(productOrder);
                product.Quantity -= item.Quantity;
            }

            order.Price = totalPrice;
            order.BillPrice = order.Price * ((100 - saleOrder.Discount) / 100);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return View(nameof(Index));
        }
    }
}
