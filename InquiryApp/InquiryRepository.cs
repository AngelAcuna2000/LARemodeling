﻿using Dapper;
using Shared.InquiryModel;
using System.Data;

namespace InquiryApp
{
    // Repository class for handling database operations related to inquiries
    public class InquiryRepository
    {
        private readonly IDbConnection _conn;

        public InquiryRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        // Retrieve all inquiries from the database
        public IEnumerable<InquiryModel> GetAllInquiries()
        {
            return _conn.Query<InquiryModel>("SELECT * FROM inquiries;");
        }

        // Retrieve a specific inquiry by its ID
        public InquiryModel GetInquiry(int id)
        {
            return _conn.QuerySingle<InquiryModel>("SELECT * FROM inquiries WHERE inquiry_id = @id", new { id = id });
        }

        // Update an existing inquiry in the database
        public void UpdateInquiry(InquiryModel inquiry)
        {
            _conn.Execute("UPDATE inquiries SET name = @name, phone = @phone, email = @email WHERE inquiry_id = @id",
             new { name = inquiry.Name, phone = inquiry.Phone, email = inquiry.Email, id = inquiry.Inquiry_ID });
        }

        // Delete an inquiry from the database
        public void DeleteInquiry(InquiryModel inquiry)
        {
            _conn.Execute("DELETE FROM inquiries WHERE inquiry_id = @id;", new { id = inquiry.Inquiry_ID });
        }
    }
}