using BTL.Controllers;
using BTL.Data;
using BTL.ViewModel;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Net.Http.Json; // Add this using statement
using Microsoft.Extensions.Configuration; // Add this using statement

namespace BTL.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7021/api"; // Configure in appsettings.json
            _httpClient.BaseAddress = new Uri(_baseApiUrl);
        }

        public async Task<ApiPagedResponse<HangHoaVM>> GetProductsAsync(ProductSearchRequest request)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrEmpty(request.Search))
            {
                queryString["Search"] = request.Search;
            }
            if (request.MaLoai.HasValue)
            {
                queryString["MaLoai"] = request.MaLoai.Value.ToString();
            }
            if (request.MinPrice.HasValue)
            {
                queryString["MinPrice"] = request.MinPrice.Value.ToString();
            }
            if (request.MaxPrice.HasValue)
            {
                queryString["MaxPrice"] = request.MaxPrice.Value.ToString();
            }
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                queryString["SortBy"] = request.SortBy;
                if (request.SortDesc.HasValue)
                {
                    queryString["SortDesc"] = request.SortDesc.Value.ToString().ToLower();
                }
            }
            if (request.SortDesc.HasValue)
            {
                queryString["SortDesc"] = request.SortDesc.Value.ToString().ToLower();
            }
            queryString["page"] = request.Page.ToString();
            queryString["pageSize"] = request.PageSize.ToString();

            var url = $"api/HangHoaAPI?{queryString}"; // Use the correct API controller name

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ApiPagedResponse<HangHoaVM>>>();
                return apiResponse?.Data;
            }
            else
            {
                // Handle error appropriately (e.g., log, throw exception, return null)
                Console.WriteLine($"Error fetching products: {response.StatusCode}");
                return null;
            }
        }

        public async Task<HangHoaVM> GetProductAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/HangHoaAPI/{id}"); // Use the correct API controller name
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<HangHoaVM>>();
                return apiResponse?.Data;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                Console.WriteLine($"Error fetching product with ID {id}: {response.StatusCode}");
                return null;
            }
        }
        public async Task<ChiTietHangHoaVM> GetProductDetailsAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/HangHoaAPI/{id}/details");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ChiTietHangHoaVM>>();
                return apiResponse?.Data;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                Console.WriteLine($"Error fetching product details with ID {id}: {response.StatusCode}");
                return null;
            }
        }
        public async Task<CartModel> GetCartAsync()
        {
            var response = await _httpClient.GetAsync($"api/CartAPI"); // Adjust route if needed
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CartModel>>();
                return apiResponse?.Data;
            }
            else
            {
                Console.WriteLine($"Error fetching cart: {response.StatusCode}");
                return null;
            }
        }

        public async Task<CartModel> AddToCartAsync(CartItemRequest item)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/CartAPI", item); // Adjust route if needed
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CartModel>>();
                return apiResponse?.Data;
            }
            else
            {
                Console.WriteLine($"Error adding to cart: {response.StatusCode}");
                // Optionally throw an exception or return a specific error CartModel
                return null;
            }
        }

        public async Task<CartModel> RemoveFromCartAsync(int maHh)
        {
            var response = await _httpClient.DeleteAsync($"api/CartAPI/{maHh}"); // Adjust route if needed
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CartModel>>();
                return apiResponse?.Data;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Console.WriteLine($"Bad request removing item {maHh} from cart: {response.StatusCode}");
                return null; // Or handle BadRequest specifically
            }
            else
            {
                Console.WriteLine($"Error removing item {maHh} from cart: {response.StatusCode}");
                return null;
            }
        }

        public async Task<ApiPagedResponse<HangHoa>> GetHangHoasAsync(ProductSearchRequest request)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["page"] = request.Page.ToString();
            queryString["pageSize"] = request.PageSize.ToString();

            var url = $"api/HangHoasAPI?{queryString}"; // Use the correct API controller name


            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ApiPagedResponse<HangHoa>>>();
                return apiResponse?.Data;
            }
            else
            {
                // Handle error appropriately (e.g., log, throw exception, return null)
                Console.WriteLine($"Error fetching products: {response.StatusCode}");
                return null;
            }
            //var response = await _httpClient.GetAsync($"/api/HangHoasAPI?page={request.Page}&pageSize={request.PageSize}");
            //if (response.IsSuccessStatusCode)
            //{
            //    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ApiPagedResponse<HangHoa>>>();
            //    return apiResponse?.Data; // Access the Data property which holds the ApiPagedResponse
            //}
            //else
            //{
            //    Console.WriteLine($"Error fetching products: {response.StatusCode}");
            //    return null;
            //}
        }

        public async Task<HangHoa?> GetHangHoasProductAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/HangHoasAPI/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<HangHoa>>();
                Console.WriteLine($"API Response for ID {id}: {System.Text.Json.JsonSerializer.Serialize(apiResponse)}"); // Add this line
                return apiResponse?.Data;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                Console.WriteLine($"Error fetching product with ID {id}: {response.StatusCode}");
                return null;
            }
        }

        public async Task<ApiResponse<HangHoa?>> CreateProductAsync(HangHoa product)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/HangHoasAPI", product);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<HangHoa?>>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse<HangHoa?>
                    {
                        Success = false,
                        Message = $"Error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<HangHoa?>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<HangHoa>> UpdateProductAsync(int id, HangHoa product)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/HangHoasAPI/{id}", product);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<HangHoa>>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse<HangHoa>
                    {
                        Success = false,
                        Message = $"Error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<HangHoa>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<object>> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/HangHoasAPI/{id}");
            return new ApiResponse<object>
            {
                Success = response.IsSuccessStatusCode,
                Message = response.ReasonPhrase,
                Data = null
            };
        }

        public async Task<ApiResponse<KhachHangResponseDto>> CreateAccountAsync(RegisterVM model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/KhachHangAPI/Register", model);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<KhachHangResponseDto>>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse<KhachHangResponseDto>
                    {
                        Success = false,
                        Message = $"Error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<KhachHangResponseDto>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<KhachHangResponseDto>> LoginAsync(LoginVM model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/KhachHangAPI/Login", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<KhachHangResponseDto>>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse<KhachHangResponseDto>
                    {
                        Success = false,
                        Message = $"Error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch(Exception ex)
            {
                return new ApiResponse<KhachHangResponseDto>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

    }
}
