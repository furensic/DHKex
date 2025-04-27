using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;

namespace DHKex;

public class DHKexMain {
    static void Main(string[] args) {
        // Diffie-Hellman key exchange flow
        // Public parameter operations:
        // 1. Generate p value, p is prime. done
        // 1.1 Using the Rabin Miller Primality Test, results in a number which has a probability to be a prime number. done
        // 1.1.1 Preselect random number with desired bit length. done
        // 1.1.2 Ensure chosen number is not divisible by the first few hundred pre-computed primes. done
        // 1.1.3 Apply n amount Rabin Miller Primality Test iterations. done
        var bitLength    = 1024;
        var randomNumber = MillerRabinPrime.GetNBitPrime(bitLength);
        Console.WriteLine($"Random {bitLength} bit number: {randomNumber}");
        
        
        // 2. Generate g value, g is primitive root modulo

        // Private parameter operations:
        // 1. Generate private value a (secret key) for participant 1
        // 2. Generate private value b (secret key) for participant 2

        // Math operations:
        // Combine secret key a with public values p & g resulting in value A (public key) for participant 1
        // Combine secret key b with public values p & g resulting in value B (public key) for participant 2
        // Transfer participants 1 public key A to participant B
        // Transfer participants 2 public key B to participant A

        // Calculate shared secret
        // Participant 1 combines public key B with secret key a
        // Participant 2 combines public key A with secret key b
    }

    
}