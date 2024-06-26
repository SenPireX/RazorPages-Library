﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Application.Model;

public class Library
{
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    [BsonId] [BsonRepresentation(BsonType.String)] public Guid Guid { get; private set; }
    [MaxLength(32)] public string Name { get; set; }
    public User? Manager { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    
    public Library(string name, ICollection<Book> books, ICollection<Loan> loans, User? manager = null)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Books = books;
        Loans = loans;
        Manager = manager;
    }
    
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Library() {}
    
}