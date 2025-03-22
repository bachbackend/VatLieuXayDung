// document.addEventListener("DOMContentLoaded", function () {
//     const params = new URLSearchParams(window.location.search);
//     const productId = params.get('httlProductId');

//     if (productId) {
//         fetch(`https://localhost:7171/api/Product/GetProductById/${productId}`)
//             .then(response => response.json())
//             .then(data => {
//                 console.log(data); 
//                 // Cập nhật thông tin sản phẩm
//                 document.querySelector('.elementor-heading-title').textContent = data.name;
//                 document.querySelector('.price .amount bdi').innerHTML = `<span class="woocommerce-Price-currencySymbol">$</span>8.50`; // Cập nhật giá nếu có
//                 document.querySelector('.woocommerce-product-details__short-description p').textContent = data.name;
//                 document.querySelector('.elementor-widget-twbb_product-content p').innerHTML = data.description;

//                 // Cập nhật breadcrumb với ID và Tên sản phẩm
//                 const breadcrumb = document.getElementById('breadcrumb-product');
//                 breadcrumb.textContent = `${data.id} - ${data.name}`;
//             })
//             .catch(error => console.error('Error:', error));
//     } else {
//         console.error('No product ID found in URL.');
//     }
// });

document.addEventListener('DOMContentLoaded', function () {
    const params = new URLSearchParams(window.location.search);
    const productId = params.get('pid');
    const token = localStorage.getItem('accessToken');  // Lấy token từ localStorage
    const authLinks = document.getElementById('auth-links');
    const authLinks1 = document.getElementById('auth-links1');

    if (token) {
        const payload = parseJwt(token);  // Giải mã token

        if (payload && payload.unique_name) {
            // Nếu giải mã thành công và có unique_name, hiển thị lời chào
            authLinks.innerHTML = `
                        <div style="display: flex; align-items: center; gap: 15px;">
                            <a href="Orders.html" style="font-weight: bold; color: black; text-decoration: none;">
                                <strong>Hello, ${payload.unique_name}!</strong>
                            </a>
                            <button onclick="logout()" title="Đăng xuất" class="logout-btn"  
                                style="border-radius: 8px; padding: 10px 20px; background-color: #ff4d4d; color: white; border: none; cursor: pointer;">
                                <i class="fas fa-sign-out-alt"></i>
                            </button>
                        </div>

                    `;
        } else {
            // Nếu token không hợp lệ, hiển thị lại nút đăng nhập/đăng ký
            showLoginButtons();
        }
    } else {
        // Nếu không có token, hiển thị nút đăng nhập/đăng ký
        showLoginButtons();
    }

    if (token) {
        const payload = parseJwt(token);  // Giải mã token

        if (payload && payload.unique_name) {
            // Nếu giải mã thành công và có unique_name, hiển thị lời chào
            authLinks1.innerHTML = `
                        <div style="display: flex; align-items: center; gap: 15px;">
                            <a href="Orders.html" style="font-weight: bold; color: black; text-decoration: none;">
                                <strong>Hello, ${payload.unique_name}!</strong>
                            </a>
                            <button onclick="logout()" title="Đăng xuất" class="logout-btn"  
                                style="border-radius: 8px; padding: 10px 20px; background-color: #ff4d4d; color: white; border: none; cursor: pointer;">
                                <i class="fas fa-sign-out-alt"></i>
                            </button>
                        </div>

                    `;
        } else {
            // Nếu token không hợp lệ, hiển thị lại nút đăng nhập/đăng ký
            showLoginButtons();
        }
    } else {
        // Nếu không có token, hiển thị nút đăng nhập/đăng ký
        showLoginButtons();
    }

    if (productId) {
        console.log('Product ID:', productId);
        fetch(`https://localhost:7171/api/Product/GetProductById/${productId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                document.getElementById('product-name').textContent = data.name;
                document.getElementById('product-name-breadcrumb').textContent = data.name;

                document.getElementById('product-description').innerHTML = data.description;
                // document.getElementById('product-image').src = `https://localhost:7171/images/${data.image}`;

                // Cập nhật ảnh chính
                const productImage = document.getElementById('product-image');
                productImage.src = `https://localhost:7171/images/${data.image}`;
                productImage.setAttribute('data-large_image', `https://localhost:7171/images/${data.image}`);
                // productImage.parentElement.href = `https://localhost:7171/images/${data.image}`; // Cập nhật link khi click vào ảnh

                document.getElementById('product-sale-quantity').textContent = `Số lượng bán: ${data.saleQuantity ?? 0}`;



            })
            .catch(error => {
                console.error('Error fetching product:', error);
            });
    } else {
        alert('No product ID found in URL');
    }

    getCartCount();
    fetchRandomProducts();

    const categoryDropdown = document.getElementById("category-dropdown");

fetch("https://localhost:7171/api/Category/GetAll", {
    method: "GET",
    headers: { "Accept": "*/*" }
})
    .then(response => {
        if (!response.ok) throw new Error("Không thể lấy danh mục.");
        return response.json();
    })
    .then(categories => {
        categoryDropdown.innerHTML = ""; // Xóa dữ liệu cũ

        categories.forEach(category => {
            const listItem = document.createElement("li");
            listItem.innerHTML = `<a href="./Building-Materials.html?pcid=${category.id}"><strong>${category.name}</strong></a>`;
            categoryDropdown.appendChild(listItem);
        });
    })
    .catch(error => {
        console.error("Lỗi khi lấy danh mục:", error);
    });
});

function parseJwt(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        return JSON.parse(jsonPayload);
    } catch (error) {
        console.error("Invalid JWT Token:", error);
        return null;
    }
}

// Hiển thị nút Đăng nhập/Đăng ký
function showLoginButtons() {
    const authLinks = document.getElementById('auth-links');
    authLinks.innerHTML = `
            <a href="login.html" class="twbb-item"><strong>ĐĂNG KÝ/ĐĂNG NHẬP</strong></a>
        
    `;
}

// Đăng xuất
function logout() {
    localStorage.removeItem('accessToken');
    window.location.href = 'home.html'; // Chuyển về trang home.html
}

async function getCartCount() {
    const userId = getUserIdFromToken();
    if (!userId) {
        console.warn("Không tìm thấy userId, vui lòng đăng nhập.");
        return;
    }

    try {
        const response = await fetch(`https://localhost:7171/api/Cart/CountUniqueProducts/${userId}`, {
            method: 'GET',
            headers: {
                'accept': '*/*'
            }
        });

        if (response.ok) {
            const data = await response.json();
            console.log("Phản hồi từ API giỏ hàng:", data); // ✅ Log response body
            const count = data.uniqueProductCount;
            document.getElementById('cart-count').innerText = count;
        } else {
            console.error('Không thể lấy số lượng sản phẩm trong giỏ hàng. Mã lỗi:', response.status);
        }
    } catch (error) {
        console.error('Lỗi khi gọi API giỏ hàng:', error);
    }
}

function getUserIdFromToken() {
    const token = localStorage.getItem("accessToken");
    if (!token) return null;

    try {
        const payload = JSON.parse(atob(token.split(".")[1]));
        return payload.nameid;
    } catch (error) {
        console.error("Lỗi giải mã token:", error);
        return null;
    }
}




// Gọi hàm khi trang được tải
// window.onload = fetchRandomProducts;


