using System;

namespace ClothesResell
{
    /// <summary>
    /// Manages the user's account balance for the application
    /// </summary>
    public static class UserBalance
    {
        private static decimal _balance = 100m; // Start with £100

        /// <summary>
        /// Gets the current balance
        /// </summary>
        public static decimal GetBalance()
        {
            return _balance;
        }

        /// <summary>
        /// Sets the balance to a specific amount
        /// </summary>
        public static void SetBalance(decimal amount)
        {
            _balance = amount;
        }

        /// <summary>
        /// Deducts an amount from the balance (for purchases)
        /// </summary>
        public static bool DeductBalance(decimal amount)
        {
            if (amount <= 0)
                return false;

            if (_balance >= amount)
            {
                _balance -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an amount to the balance (for sales)
        /// </summary>
        public static void AddBalance(decimal amount)
        {
            if (amount > 0)
            {
                _balance += amount;
            }
        }

        /// <summary>
        /// Resets balance to the starting amount
        /// </summary>
        public static void ResetBalance()
        {
            _balance = 100m;
        }
    }
}
