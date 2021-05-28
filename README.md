# Overview
This is a demonstration of a partial Grocery Store REST API.
The API is documented with Swagger at the root of the web site (/index.html).

# Version
This document describes API version 1.0.
To use this API version, supply request header `Api-Version: 1.0` in every call to the API.

# Authentication
The API requires username/password login authentication and uses cookies to persist a successful login.
- **URL**<br/>
/api/login/authenticate
- **Method**<br/>
	POST
- **JSON Payload**
~~~~
	{
		"username": "user",
		"password": "a733tc0d3r"
	}
~~~~
- **Success Response**
	* **Code**: 200 OK
- **Error Response**
	* **Code**: 401 UNAUTHORIZED
- **Sample response**
~~~~
	HTTP/1.1 200 OK
	Cache-Control: no-cache
	Pragma: no-cache
	Transfer-Encoding: chunked
	Content-Type: application/json; charset=utf-8
	Expires: Thu, 01 Jan 1970 00:00:00 GMT
	Server: Microsoft-IIS/10.0
	Set-Cookie: GroceryStoreAPI=CfDJ8FFyHTmiRWtAlbAMEB-zXbtOwYJy0vN0DB5SUH_ZhHQK8knk4PgUR8woJNPr6nBPfhXgzJuSe43Auh4Sv0W1pG0siUWQ0JVO209lcRKng63xatm0SRSDLAMsPX-0fCHN9mogDfffESkyy8mVtqYJWjuhRblWvXylQYifJS-ZAdfpyaCFqnEBMmLZKXu2HhhdPCiERoEgnpeMVYsI-IrcdZQt4w7zsgOo7rwXb2fqElhvg8hqqhoxX5MDYgPYbrqlwIAKqsDNiEpt3Mg6iNToRy41x9tVweELrVGg7giX0zgF5ZBvHcagqSR-QjhdNUwcxk7zGsl_2Nj9SJzhZKu2F4PNWExzDkrBNe2ZaPKPlp1-5gP9xCPEQpZ2PR3YbMkqLhZiUHqtAiQr5jW0KA3bc7u9BF1ydIlVx9voa6vHcMs1niohVIR06jwtjzFyqfPNWPKKKVgRUQFGEU-mg9-MupE; expires=Fri, 11 Jun 2021 13:46:26 GMT; path=/; samesite=lax; httponly
	api-supported-versions: 1.0
	X-Powered-By: ASP.NET
	Date: Fri, 28 May 2021 13:46:26 GMT

2a
{"id":1,"username":"user","password":null}
0`
~~~~

# General responses
**Redirect**<br/>
	* **Code**: 302 REDIRECTED - The session is not logged in, the URL redirects to a login URL.<br/>
**Unauthorized**<br/>
	* **Code**: 401 UNAUTHORIZED - The session is not logged in.<br/>
**Internal Server Error**<br/>
	* **Code**: 500 INTERNAL SERVER ERROR - Consult the server logs to determine the cause.

# Supported API

**API listing all customers**
----
- **URL**<br />
	/api/customers
- **Method**<br/> 
	GET
 - **Query Parameters**<br/>
   * **Optional:**<br/>
	 `fromRow=[integer]` - The ordinal of the first row to retrieve<br/>
	 `pageSize=[integer]` - The number of rows to retrieve<br/>
	 `sortBy=[string]` - The field name by which to sort response rows<br/>
- **Success Response**
	* **Code** 200 OK
- **Error Responses**
	* **Code** 400 BAD REQUEST - The call failed, examine the response body for an explanation.
	* **Code** 204 NO CONTENT - The database contains no (matching) data.
- **Response**
	- A JSON document containing requested customers, e.g.
~~~~
[{"id":1,"name":"Bob"},{"id":2,"name":"Mary"},{"id":3,"name":"Joe"}]
~~~~

 **API retrieving a customer**
 ----
 - **URL**<br/>
	/api/customers
 - **Query Parameters**<br/>
   * **Required:**<br/>
	 `id=[integer]` - The customer id
- **Success Response**
	* **Code** 200 OK
- **Error Responses**
	* **Code** 400 BAD REQUEST - The call failed, examine the response body for an explanation. 
	* **Code** 404 NOT FOUND - The customer for the requested id does not exist in the database.
- **Response**
	- A JSON document containing the requested customer, e.g.
~~~~
{"id":1,"name":"Bob"}
~~~~

**API adding a customer**
---
- **URL**<br/>
	/api/customers
- **Method**<br/>
	PUT
- **JSON Payload**<br/>
~~~~
{
  "id": 0
  "name": "some name"
}
~~~~
- **Success Response**
	* **Code** 201 CREATED
- **Error Responses**
	* **Code** 400 BAD REQUEST - The call failed, examine the response body for an explanation. 
 - **Response**
	- A JSON document containing the newly added customer, e.g.
~~~~
{"id":4,"name":"Emma"}
~~~~

**API updating a customer**
---
- **URL**<br/>
	/api/customers
- **Method**<br/>
	POST
- **JSON Payload**<br/>
~~~~
{
  "id": [integer]
  "name": "[string]"
}
~~~~
- **Success Response**
	* **Code** 200 OK
- **Error Responses**
	* **Code** 400 BAD REQUEST - The call failed, examine the response body for an explanation. 
- **Response**
	- A JSON document containing the updated customer, e.g.
~~~~
{"id":1,"name":"Dave"}
~~~~

# Optional Request Parameters
- **Request Timeout**
	* `timeout=[integer]` - Sets the request timeout to the value in milliseconds. After the timeout expires, the query will terminate.

# Implementation Notes
 - Due to time constraints, unit tests are meant as a demonstration of tools and techniques (xUnit and Moq). Test coverage is not complete.

