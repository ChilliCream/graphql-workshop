using System;

namespace Demo.Accounts
{
    public record User(int Id, string Name, DateTime Birthdate, string Username);
}