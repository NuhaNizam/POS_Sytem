using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using POSWPF.Models;

namespace POSWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _productCode;
        private decimal _total;
        private ObservableCollection<Product> _cartItems;
        
        public MainViewModel()
        {
            CartItems = new ObservableCollection<Product>();
            AddToCartCommand = new RelayCommand(AddToCart, CanAddToCart);
            
            // Sample products (in a real app, this would come from a database)
            SampleProducts = new[]
            {
                new Product { Code = "001", Name = "Product 1", Price = 10.99m },
                new Product { Code = "002", Name = "Product 2", Price = 15.99m },
                new Product { Code = "003", Name = "Product 3", Price = 20.99m }
            };
        }

        public string ProductCode
        {
            get => _productCode;
            set
            {
                if (SetProperty(ref _productCode, value))
                {
                    (AddToCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal Total
        {
            get => _total;
            private set => SetProperty(ref _total, value);
        }

        public ObservableCollection<Product> CartItems
        {
            get => _cartItems;
            private set => SetProperty(ref _cartItems, value);
        }

        public ICommand AddToCartCommand { get; }

        private Product[] SampleProducts { get; }

        private bool CanAddToCart() => !string.IsNullOrWhiteSpace(ProductCode);

        private void AddToCart()
        {
            var product = SampleProducts.FirstOrDefault(p => p.Code == ProductCode);
            if (product != null)
            {
                var cartItem = CartItems.FirstOrDefault(i => i.Code == product.Code);
                if (cartItem != null)
                {
                    cartItem.Quantity++;
                }
                else
                {
                    CartItems.Add(new Product
                    {
                        Code = product.Code,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = 1
                    });
                }

                UpdateTotal();
                ProductCode = string.Empty;
            }
        }

        private void UpdateTotal()
        {
            Total = CartItems.Sum(item => item.Total);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
} 