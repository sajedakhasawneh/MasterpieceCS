using System.Diagnostics;
using System.Security.Claims;
using Masterpiece.Models;
using Masterpiece.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Masterpiece.Controllers
{
    public class paymentController : Controller
    {
        private readonly MyDbContext _context;

        public paymentController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Cart()
        {
            int? userID = HttpContext.Session.GetInt32("UserId");
         
            if (userID == null)
            {
                return RedirectToAction("Register", "User"); // Or any other logic for unauthenticated users
            }
            var cart = _context.Carts
                               .Include(c => c.CartItems)
                               .ThenInclude(ci => ci.Product)
                               .FirstOrDefault(c => c.UserId == userID.Value);

            if (cart == null || !cart.CartItems.Any())
            {
                // Return empty cart view
                return View(new CartsVM());
            }

            // Map to ViewModel
            var cartItemsVM = cart.CartItems.Select(ci => new CartItemViewModel
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ImageUrl = ci.Product.ImageUrl ?? "/images/default.png", // fallback
                Price = ci.UnitPrice,
                Quantity = ci.Quantity
            }).ToList();

            var viewModel = new CartsVM
            {
                CartItems = cartItemsVM
            };

            return View(viewModel);
        }


        

        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int newQuantity)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);

            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
                _context.SaveChanges();
            }

            return RedirectToAction("Cart");
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        public IActionResult RemoveItem(int cartItemId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Cart", "payment");
        }

        public IActionResult addItemToCart(int productId, int quantity)
        {
            // Get the logged-in user's ID from session
            int? userID = HttpContext.Session.GetInt32("UserId");

            if (userID == null)
            {
                return RedirectToAction("Register", "User"); // not logged in
            }

            // Get or create the user's cart
            var cart = _context.Carts
                               .Include(c => c.CartItems)
                               .FirstOrDefault(c => c.UserId == userID.Value);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userID.Value,
                    CreatedAt = DateTime.Now
                };
                _context.Carts.Add(cart);
                _context.SaveChanges(); // Save now to generate Cart ID
            }

            // Check if product is already in cart
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += quantity;
            }
            else
            {
                // Get product price
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    return NotFound("Product not found.");
                }

                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                _context.CartItems.Add(newItem);
            }

            _context.SaveChanges();
            return RedirectToAction("Cart", "payment"); // Go back to cart
        }

        
        public IActionResult payment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult checkout()
        {
            int? userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
            {
                return RedirectToAction("Register", "User");
            }

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userID);

            if (cart == null || cart.CartItems.Count == 0)
            {
                return BadRequest("Cart is empty or not found.");
            }

            //add to order
            var order = new Order
            {
                UserId = cart.UserId,
                CreatedAt = DateTime.Now,
                TotalPrice = cart.CartItems.Sum(item => item.Quantity * item.UnitPrice),
                //PaymentStatus = "Paid",
                Status = "Pending",
                OrderItems = cart.CartItems.Select(item => new OrderItem
                {
                    //OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            return View();
        }



        //public async Task<IActionResult> Index()
        //{
        //    int? userID = HttpContext.Session.GetInt32("UserId");

        //    if (userID == null)
        //    {
        //        return RedirectToAction("Register", "User"); // Or any other logic for unauthenticated users
        //    }
        //    var cart = await _context.Carts
        //        .Include(c => c.CartItems)
        //        .ThenInclude(ci => ci.Product)
        //        .FirstOrDefaultAsync(c => c.UserId == userID);

        //    return View(cart);
        //}



        public IActionResult checkoutView()
        {
            int? userID = HttpContext.Session.GetInt32("UserId");

            if (userID == null)
            {
                return RedirectToAction("Register", "User"); // Or any other logic for unauthenticated users
            }

            var cartItems = _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserId == userID)
                .Select(ci => new CartItemViewModel
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.UnitPrice,
                    Quantity = ci.Quantity,
                    ImageUrl = ci.Product.ImageUrl
                }).ToList();

            var vm = new CartsVM
            {
                CartItems = cartItems
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder(string firstName, string lastName, string address, string email, string paymentMethod)
        {
            int? userID = HttpContext.Session.GetInt32("UserId");

            if (userID == null)
            {
                return RedirectToAction("Register", "User"); // Or any other logic for unauthenticated users
            }

            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserId == userID)
                .ToListAsync();

            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Cart is empty.");
                return RedirectToAction("Index");
            }

            var totalPrice = cartItems.Sum(ci => ci.Quantity * ci.UnitPrice);

            // Create Order
            var order = new Order
            {
                UserId = userID.Value,
                TotalPrice = totalPrice,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Add OrderItems
            foreach (var item in cartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            // Add Payment
            _context.Payments.Add(new Payment
            {
                OrderId = order.OrderId,
                UserId = userID.Value,
                Amount = totalPrice,
                Method = paymentMethod,
                Status = "completed",
                CreatedAt = DateTime.Now
            });

            // Remove from cart
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", new { id = order.OrderId });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }



        public IActionResult shipping()
        {
            return View();
        }




        public IActionResult Index()
        {
            int? userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
                return RedirectToAction("Register", "User");

            var cartItems = _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserId == userID)
                .Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.UnitPrice,
                    Quantity = ci.Quantity,
                    ImageUrl = ci.Product.ImageUrl
                }).ToList();

            var vm = new CheckoutViewModel
            {
                CartItems = cartItems
            };

            return View(vm);
        }

        //[HttpPost]
        //public async Task<IActionResult> Index(CheckoutViewModel model)
        //{
        //    int? userID = HttpContext.Session.GetInt32("UserId");
        //    if (userID == null)
        //        return RedirectToAction("Register", "User");

        //    var cartItems = _context.CartItems
        //        .Include(ci => ci.Product)
        //        .Where(ci => ci.Cart.UserId == userID)
        //        .ToList();

        //    if (!ModelState.IsValid)
        //    {
        //        model.CartItems = cartItems.Select(ci => new CartItemViewModel
        //        {
        //            ProductId = ci.ProductId,
        //            ProductName = ci.Product.Name,
        //            Price = ci.UnitPrice,
        //            Quantity = ci.Quantity,
        //            ImageUrl = ci.Product.ImageUrl
        //        }).ToList();

        //        return View(model);
        //    }

        //    // Voucher logic
        //    decimal discount = 0;
        //    if (!string.IsNullOrEmpty(model.VoucherCode))
        //    {
        //        var voucher = _context.Vouchers
        //        .FirstOrDefault(v => v.Code == model.VoucherCode && v.ExpiryDate > DateTime.Now);

        //        if (voucher != null)
        //            discount = voucher.DiscountValue;

        //    }

        //    //decimal total = cartItems.Sum(ci => ci.Quantity * ci.UnitPrice) - discount;

        //    // Create Order
        //    var order = new Order
        //    {
        //        UserId = userID.Value,
        //        //TotalPrice = total,
        //        Status = "Pending",
        //        CreatedAt = DateTime.Now
        //    };
        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    foreach (var item in cartItems)
        //    {
        //        _context.OrderItems.Add(new OrderItem
        //        {
        //            OrderId = order.OrderId,
        //            ProductId = item.ProductId,
        //            Quantity = item.Quantity,
        //            UnitPrice = item.UnitPrice
        //        });
        //    }

        //    _context.Payments.Add(new Payment
        //    {
        //        OrderId = order.OrderId,
        //        UserId = userID.Value,
        //        //Amount = total,
        //        Method = model.PaymentMethod,
        //        Status = "completed",
        //        CreatedAt = DateTime.Now
        //    });

        //    _context.CartItems.RemoveRange(cartItems);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Confirmation", new { id = order.OrderId });
        //}




        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            int? userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
                return RedirectToAction("Register", "User");

            // Fetch cart items
            var cartItems = _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserId == userID)
                .ToList();

            // Repopulate view if validation fails
            if (!ModelState.IsValid)
            {
                model.CartItems = cartItems.Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.UnitPrice,
                    Quantity = ci.Quantity,
                    ImageUrl = ci.Product.ImageUrl
                }).ToList();

                return View(model);
            }

            // Voucher logic
            decimal discount = 0;
            if (!string.IsNullOrEmpty(model.VoucherCode))
            {
                var voucher = _context.Vouchers
                    .FirstOrDefault(v => v.Code == model.VoucherCode && v.ExpiryDate > DateTime.Now);

                if (voucher != null)
                    discount = voucher.DiscountValue;
            }

            // Calculate total
            decimal totalBeforeDiscount = cartItems.Sum(ci => ci.Quantity * ci.UnitPrice);
            decimal totalAfterDiscount = totalBeforeDiscount - discount;

            // Create Order
            var order = new Order
            {
                UserId = userID.Value,
                TotalPrice = totalAfterDiscount,
                Status = "Pending", // You can adjust this based on payment later
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create Order Items
            foreach (var item in cartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            // Determine payment status based on selected method
            string paymentStatus = model.PaymentMethod switch
            {
                "Cash" => "pending",     // To be collected upon delivery
                "Card" => "completed",   // Assuming card details are valid and accepted
                 // Will be completed via PayPal redirect
                _ => "unknown"
            };

            // Create Payment record
            _context.Payments.Add(new Payment
            {
                OrderId = order.OrderId,
                UserId = userID.Value,
                Amount = totalAfterDiscount,
                Method = model.PaymentMethod,
                Status = paymentStatus,
                CreatedAt = DateTime.Now
            });

            // Clear the user's cart
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // Optional: Redirect to PayPal if chosen
            if (model.PaymentMethod == "PayPal")
            {
                // You would call your PayPal service here and redirect to their site
                // For now, just simulate the behavior
                TempData["PayPalRedirectNote"] = "You would now be redirected to PayPal.";
                return RedirectToAction("Confirmation", new { id = order.OrderId });
            }

            return RedirectToAction("Confirmation", new { id = order.OrderId });
        }




        [HttpPost]
        public IActionResult ApplyVoucher(string voucherCode)
        {
            Debug.WriteLine("ApplyVoucher action triggered!");

            int? userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
                return Json(new { success = false, message = "User not logged in." });

            // Get the cart items
            var cartItems = _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserId == userID)
                .ToList();

            // Calculate total before discount
            decimal totalBeforeDiscount = cartItems.Sum(ci => ci.Quantity * ci.UnitPrice);

        


            decimal discount = 0;
            if (!string.IsNullOrEmpty(voucherCode))
            {
                var voucher = _context.Vouchers
                    .FirstOrDefault(v => v.Code == voucherCode && v.ExpiryDate > DateTime.Now);

                if (voucher != null)
                    discount = voucher.DiscountValue;

                if (voucher == null || voucher.ExpiryDate < DateTime.Now)
                {
                    discount = 0;
                }
            }

            decimal totalAfterDiscount = totalBeforeDiscount - discount;

            // Return the updated totals
            return Json(new
            {
                success = true,
                totalBeforeDiscount = totalBeforeDiscount,
                discountAmount = discount,
                totalAfterDiscount = totalAfterDiscount
            });
        }


    }
}

