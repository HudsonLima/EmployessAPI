using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EmployessAPI.EFModel;
using EmployessAPI.Models;
using Newtonsoft.Json;
using System.Web;

namespace EmployessAPI.Controllers
{
    public class EmployeeController : ApiController
    {
        private EmployeesManagerEntities db = new EmployeesManagerEntities();

        // GET: api/Employees/?&page_size=1&page=1
        public IEnumerable<employee> Getemployees([FromUri]PagingParameterModel pagingparametermodel)
        {

            try
            {

                // Return List of Employees  
                var source = (from employees in db.employees.
                                OrderBy(a => a.Name)
                              select employees).AsQueryable();

                // Get's No of Rows Count   
                int count = source.Count();

                // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                int CurrentPage = pagingparametermodel.page;

                // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                int PageSize = pagingparametermodel.page_size;

                // Display TotalCount to Records to User  
                int TotalCount = count;

                // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                // Returns List of Employees after applying Paging   
                var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                // if CurrentPage is greater than 1 means it has previousPage  
                var previousPage = CurrentPage > 1 ? "Yes" : "No";

                // if TotalPages is greater than CurrentPage means it has nextPage  
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                // Object which we are going to send in header   
                var paginationMetadata = new
                {
                    totalCount = TotalCount,
                    pageSize = PageSize,
                    currentPage = CurrentPage,
                    totalPages = TotalPages,
                    previousPage,
                    nextPage
                };

                // Setting Header  
                HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
                // Returing List of Employees Collections  
                return items;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: api/Employee/5
        [ResponseType(typeof(employee))]
        public IHttpActionResult Getemployee(int id)
        {
            try
            {

                employee employee = db.employees.Find(id);
                if (employee == null)
                {
                    return NotFound();
                }

                return Ok(employee);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // PUT: api/Employee/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putemployee(int id, employee employee)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != employee.Id)
                {
                    return BadRequest();
                }

                db.Entry(employee).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!employeeExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // POST: api/Employee
        [ResponseType(typeof(employee))]
        public IHttpActionResult Postemployee(employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.employees.Add(employee);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = employee.Id }, employee);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // DELETE: api/Employee/5
        [ResponseType(typeof(employee))]
        public IHttpActionResult Deleteemployee(int id)
        {
            try
            {

                employee employee = db.employees.Find(id);
                if (employee == null)
                {
                    return NotFound();
                }

                db.employees.Remove(employee);
                db.SaveChanges();

                return Ok(employee);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool employeeExists(int id)
        {
            return db.employees.Count(e => e.Id == id) > 0;
        }
    }
}