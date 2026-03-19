using Portehobe.src.PorteHobe.Infrastructure;
using PorteHobe.Domain.Entities;
using System.Collections.Generic;

namespace PorteHobe.API.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            // Don't seed if data already exists
            if (context.Resources.Any())
                return;

            var resources = new List<Resource>
            {
                new Resource
                {
                    Title = "Merge Sort Algorithm Explained",
                    Description = "Complete tutorial on merge sort with step-by-step implementation and complexity analysis.",
                    ResourceType = "Video",
                    Subject = "Computer Science",
                    AuthorOrChannel = "CS Fundamentals",
                    ThumbnailUrl = "https://img.youtube.com/vi/example1/hqdefault.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=example1",
                    Duration = "15:42",
                    ViewCount = 125000,
                    Rating = 4.8,
                    DifficultyLevel = "Intermediate",
                    Tags = "sorting,algorithms,divide-conquer",
                    IsFeatured = true
                },
                new Resource
                {
                    Title = "Calculus: Derivatives and Limits",
                    Description = "Master the fundamentals of calculus with practical examples and visual explanations.",
                    ResourceType = "Video",
                    Subject = "Mathematics",
                    AuthorOrChannel = "Math Master",
                    ThumbnailUrl = "https://img.youtube.com/vi/example2/hqdefault.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=example2",
                    Duration = "22:15",
                    ViewCount = 89000,
                    Rating = 4.9,
                    DifficultyLevel = "Beginner",
                    Tags = "calculus,derivatives,limits",
                    IsFeatured = true
                },
                new Resource
                {
                    Title = "Understanding Quantum Mechanics",
                    Description = "A comprehensive guide to quantum mechanics principles and wave-particle duality.",
                    ResourceType = "Article",
                    Subject = "Physics",
                    AuthorOrChannel = "Physics Today",
                    ThumbnailUrl = "",
                    VideoUrl = "",
                    Duration = "8 min read",
                    ViewCount = 45000,
                    Rating = 4.7,
                    DifficultyLevel = "Advanced",
                    Tags = "quantum,mechanics,physics",
                    IsFeatured = true
                },
                new Resource
                {
                    Title = "Linear Algebra - Vectors and Matrices",
                    Description = "Learn the basics of vectors, matrices, and linear transformations.",
                    ResourceType = "Video",
                    Subject = "Mathematics",
                    AuthorOrChannel = "3Blue1Brown",
                    ThumbnailUrl = "https://img.youtube.com/vi/example4/hqdefault.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=example4",
                    Duration = "18:30",
                    ViewCount = 200000,
                    Rating = 4.9,
                    DifficultyLevel = "Beginner",
                    Tags = "linear-algebra,vectors,matrices",
                    IsFeatured = true
                },
                new Resource
                {
                    Title = "Organic Chemistry Basics",
                    Description = "Introduction to organic chemistry, carbon compounds, and functional groups.",
                    ResourceType = "Video",
                    Subject = "Chemistry",
                    AuthorOrChannel = "Chemistry Hub",
                    ThumbnailUrl = "https://img.youtube.com/vi/example5/hqdefault.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=example5",
                    Duration = "25:10",
                    ViewCount = 67000,
                    Rating = 4.5,
                    DifficultyLevel = "Beginner",
                    Tags = "organic,chemistry,carbon",
                    IsFeatured = true
                },
                new Resource
                {
                    Title = "Data Structures: Binary Trees",
                    Description = "Complete guide to binary trees, BST, traversals and common operations.",
                    ResourceType = "Video",
                    Subject = "Computer Science",
                    AuthorOrChannel = "CS Fundamentals",
                    ThumbnailUrl = "https://img.youtube.com/vi/example6/hqdefault.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=example6",
                    Duration = "20:45",
                    ViewCount = 150000,
                    Rating = 4.7,
                    DifficultyLevel = "Intermediate",
                    Tags = "data-structures,binary-tree,BST",
                    IsFeatured = true
                }
            };

            context.Resources.AddRange(resources);
            context.SaveChanges();
        }
    }
}