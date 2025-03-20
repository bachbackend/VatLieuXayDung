document.addEventListener('DOMContentLoaded', function () {
    fetchProducts(1); // Gọi API khi trang được load, bắt đầu từ trang 1
});

const itemsPerPage = 12;  // Số sản phẩm mỗi trang
let currentPage = 1;

// Hàm để render các nút phân trang
function renderPaginationButtons(totalPages) {
    const paginationElement = document.getElementById('pagination');
    paginationElement.innerHTML = ''; // Xóa các nút phân trang cũ

    for (let i = 1; i <= totalPages; i++) {
        const button = document.createElement('button');
        button.textContent = i;
        button.classList.add('btn', 'btn-outline-primary', 'me-2');

        // Thêm lớp 'active' cho trang hiện tại
        if (i === currentPage) {
            button.classList.remove('btn-outline-primary');
            button.classList.add('btn-primary', 'active');
        }

        // Thêm sự kiện click để chuyển trang và cập nhật nút active
        button.addEventListener('click', () => {
            currentPage = i; // Cập nhật trang hiện tại
            fetchProducts(currentPage); // Gọi lại API để lấy dữ liệu trang mới
        });

        paginationElement.appendChild(button);
    }
}


// Hàm lấy sản phẩm từ API
function fetchProducts(pageNumber) {
    const keyword = document.getElementById('product-name-search').value.trim();
    const categoryId = document.getElementById('product-category').value;
    const params = new URLSearchParams();
                    if (keyword) {
                        params.append('name', keyword);
                    }
                    if (categoryId) {
                        params.append('categoryId', categoryId);
                    }

                    if (params.toString()) {
                        url += `&${params.toString()}`;
                    }
    fetch(`https://localhost:7171/api/Product/GetAllProduct?pageNumber=${pageNumber}`, {
        method: 'GET',
        headers: {
            'accept': '*/*'
        }
    })
    .then(response => response.json())
    .then(data => {
        currentPage = pageNumber;  // Cập nhật trang hiện tại
        displayProducts(data.products);
        renderPaginationButtons(data.paging.totalPageCount);  // Render các nút phân trang sau khi lấy dữ liệu
    })
    .catch(error => console.error('Error fetching products:', error));
}

// Hàm hiển thị sản phẩm ra giao diện
function displayProducts(products) {
    const productList = document.getElementById('product-list');
    productList.innerHTML = ''; // Xóa các sản phẩm cũ

    products.forEach(product => {
        console.log(product);
        const productItem = document.createElement('li');
        productItem.className = 'product type-product instock product-type-simple';

        productItem.innerHTML = `
            <a href="Product-Detail.html?pid=${product.id}" class="woocommerce-LoopProduct-link woocommerce-loop-product__link">
                <div class="twbb-image-container" style="aspect-ratio: 1/1;">
                    <img src="https://localhost:7171/images/${product.image}" alt="${product.name}" class="attachment-woocommerce_thumbnail size-woocommerce_thumbnail" decoding="async">
                </div>
                <h2 class="woocommerce-loop-product__title">${product.name}</h2>
                <span class="price">
                    <span class="woocommerce-Price-amount amount">
                        <a class="link" href="https://zalo.me/0773026042">
                            <bdi>Giá: Liên hệ</bdi>
                        </a>
                    </span>
                </span>
            </a>
            
        `;

        productList.appendChild(productItem);
    });
}

document.getElementById('searchButton').addEventListener('click', () => {
    currentPage = 1; // Reset về trang đầu tiên
    fetchProducts(currentPage);
});

