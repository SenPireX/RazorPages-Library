using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Application.Model;

public enum Usertype { Owner = 1, Admin }

public class User
{
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    [BsonId] [BsonRepresentation(BsonType.String)] public Guid Guid { get; private set; }
    [MaxLength(255)] public string Username { get; set; }
    [MaxLength(44)] public string Salt { get; set; } /* 256 bit Hash as base64 */
    [MaxLength(88)] public string PasswordHash { get; set; } /* 512 bit SHA512 Hash as base64 */
    public Usertype Usertype { get; set; }
    public ICollection<Library> Libraries { get; } = new List<Library>();
    
    public User(string username, string salt, string passwordHash, Usertype usertype)
    {
        Username = username;
        Salt = salt;
        PasswordHash = passwordHash;
        Usertype = usertype;
        Guid = Guid.NewGuid();
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected User() {}
    
}