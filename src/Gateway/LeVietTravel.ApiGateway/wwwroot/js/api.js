/**
 * LeViet Travel - API Service với hỗ trợ JWT, Refresh Token Rotation tự động
 */

const API_BASE = ''; // Rỗng vì đang chạy cùng Gateway trên http://localhost:5000

const TokenStorage = {
    getAccessToken() {
        return localStorage.getItem('access_token');
    },
    getRefreshToken() {
        return localStorage.getItem('refresh_token');
    },
    getUser() {
        const u = localStorage.getItem('current_user');
        return u ? JSON.parse(u) : null;
    },
    setTokens(accessToken, refreshToken, user) {
        if (accessToken) localStorage.setItem('access_token', accessToken);
        if (refreshToken) localStorage.setItem('refresh_token', refreshToken);
        if (user) localStorage.setItem('current_user', JSON.stringify(user));
    },
    clear() {
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('current_user');
    }
};

let isRefreshing = false;
let refreshSubscribers = [];

function onRefreshed(token) {
    refreshSubscribers.map(cb => cb(token));
    refreshSubscribers = [];
}

function addRefreshSubscriber(cb) {
    refreshSubscribers.push(cb);
}

/**
 * Hàm gọi API dùng chung, tự động bắt lỗi 401 và gọi Refresh Token để xoay vòng token
 */
async function apiFetch(endpoint, options = {}) {
    const url = endpoint.startsWith('http') ? endpoint : `${API_BASE}${endpoint}`;
    options.headers = options.headers || {};
    
    // Đính kèm Access Token nếu có
    const accessToken = TokenStorage.getAccessToken();
    if (accessToken && !options.headers['Authorization']) {
        options.headers['Authorization'] = `Bearer ${accessToken}`;
    }

    if (!options.headers['Content-Type'] && !(options.body instanceof FormData)) {
        options.headers['Content-Type'] = 'application/json';
    }

    let response = await fetch(url, options);

    // Xử lý khi Token hết hạn (401 Unauthorized)
    if (response.status === 401 && TokenStorage.getRefreshToken() && !endpoint.includes('/refresh-token') && !endpoint.includes('/login')) {
        if (!isRefreshing) {
            isRefreshing = true;
            try {
                console.log('🔄 Access Token hết hạn, đang gọi Refresh Token để gia hạn...');
                const refreshRes = await fetch(`${API_BASE}/api/auth/refresh-token`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ refreshToken: TokenStorage.getRefreshToken() })
                });

                const refreshData = await refreshRes.json();

                if (refreshRes.ok && refreshData.success) {
                    const { accessToken: newAccess, refreshToken: newRefresh, user } = refreshData.data;
                    TokenStorage.setTokens(newAccess, newRefresh, user);
                    console.log('✅ Gia hạn Token thành công!');
                    onRefreshed(newAccess);
                } else {
                    console.warn('❌ Refresh Token không hợp lệ hoặc đã hết hạn.');
                    TokenStorage.clear();
                    window.dispatchEvent(new Event('auth:unauthorized'));
                    throw new Error('Phiên đăng nhập đã hết hạn.');
                }
            } catch (err) {
                TokenStorage.clear();
                window.dispatchEvent(new Event('auth:unauthorized'));
                throw err;
            } finally {
                isRefreshing = false;
            }
        }

        // Chờ token mới rồi retry lại request cũ
        const retryToken = await new Promise(resolve => addRefreshSubscriber(resolve));
        options.headers['Authorization'] = `Bearer ${retryToken}`;
        response = await fetch(url, options);
    }

    const data = await response.json().catch(() => ({ success: response.ok, message: response.statusText }));

    if (!response.ok) {
        const errorMsg = data.message || `Lỗi yêu cầu (${response.status})`;
        throw new Error(errorMsg);
    }

    return data;
}

// Đối tượng API chứa các chức năng cho giao diện
const API = {
    // Auth
    async login(email, password) {
        const res = await apiFetch('/api/auth/login', {
            method: 'POST',
            body: JSON.stringify({ email, password })
        });
        if (res.success && res.data) {
            TokenStorage.setTokens(res.data.accessToken, res.data.refreshToken, res.data.user);
        }
        return res;
    },

    async register(fullName, email, password, phoneNumber) {
        const res = await apiFetch('/api/auth/register', {
            method: 'POST',
            body: JSON.stringify({ fullName, email, password, phoneNumber })
        });
        if (res.success && res.data) {
            TokenStorage.setTokens(res.data.accessToken, res.data.refreshToken, res.data.user);
        }
        return res;
    },

    async getMe() {
        return apiFetch('/api/auth/me');
    },

    async refreshToken() {
        const rToken = TokenStorage.getRefreshToken();
        if (!rToken) throw new Error('Không có Refresh Token trong bộ nhớ.');
        return apiFetch('/api/auth/refresh-token', {
            method: 'POST',
            body: JSON.stringify({ refreshToken: rToken })
        });
    },

    async logout() {
        try {
            const rToken = TokenStorage.getRefreshToken();
            if (rToken) {
                await apiFetch('/api/auth/revoke-token', {
                    method: 'POST',
                    body: JSON.stringify({ refreshToken: rToken })
                });
            }
        } catch (e) {
            console.error('Lỗi khi thu hồi token:', e);
        } finally {
            TokenStorage.clear();
        }
    },

    async testAdminRole() {
        return apiFetch('/api/auth/admin-only');
    },

    // Tours
    async getTours(params = {}) {
        const query = new URLSearchParams(params).toString();
        return apiFetch(`/api/tours?${query}`);
    },

    async getTourById(id) {
        return apiFetch(`/api/tours/${id}`);
    },

    async getCategories() {
        return apiFetch('/api/tours/categories');
    },

    async createTour(tourData) {
        return apiFetch('/api/tours', {
            method: 'POST',
            body: JSON.stringify(tourData)
        });
    },

    async deleteTour(id) {
        return apiFetch(`/api/tours/${id}`, {
            method: 'DELETE'
        });
    },

    // Bookings
    async createBooking(bookingData) {
        return apiFetch('/api/bookings', {
            method: 'POST',
            body: JSON.stringify(bookingData)
        });
    },

    async getMyBookings() {
        return apiFetch('/api/bookings/my-bookings');
    },

    async getBookingById(id) {
        return apiFetch(`/api/bookings/${id}`);
    },

    async getAllBookings(status = '') {
        const q = status ? `?status=${encodeURIComponent(status)}` : '';
        return apiFetch(`/api/bookings${q}`);
    },

    async confirmBooking(id) {
        return apiFetch(`/api/bookings/${id}/confirm`, {
            method: 'PUT'
        });
    },

    async cancelBooking(id) {
        return apiFetch(`/api/bookings/${id}/cancel`, {
            method: 'PUT'
        });
    }
};

// Hiển thị Toast thông báo
function showToast(message, type = 'success') {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `<span>${type === 'success' ? '✔' : '✖'}</span> ${message}`;
    container.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 4000);
}
