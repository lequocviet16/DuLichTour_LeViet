/**
 * Logic giao diện trang chủ LeViet Travel
 */

document.addEventListener('DOMContentLoaded', () => {
    initAuthUI();
    loadCategories();
    loadTours();

    window.addEventListener('auth:unauthorized', () => {
        initAuthUI();
        showToast('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.', 'error');
    });
});

function initAuthUI() {
    const user = TokenStorage.getUser();
    const authSection = document.getElementById('auth-section');
    const navMyBookings = document.getElementById('nav-my-bookings');
    const navAdmin = document.getElementById('nav-admin');

    if (user) {
        if (navMyBookings) navMyBookings.style.display = 'block';
        if (navAdmin) navAdmin.style.display = user.role === 'ADMIN' ? 'block' : 'none';

        authSection.innerHTML = `
            <div style="display: flex; align-items: center; gap: 0.8rem;">
                <span style="font-size: 0.9rem; font-weight: 600;">👋 ${user.fullName} <span class="badge ${user.role === 'ADMIN' ? 'badge-danger' : 'badge-success'}">${user.role}</span></span>
                <button class="btn btn-outline btn-sm" onclick="handleLogout()">Đăng Xuất</button>
            </div>
        `;
    } else {
        if (navMyBookings) navMyBookings.style.display = 'none';
        if (navAdmin) navAdmin.style.display = 'none';

        authSection.innerHTML = `
            <button class="btn btn-outline btn-sm" onclick="openAuthModal('login')">Đăng Nhập</button>
            <button class="btn btn-primary btn-sm" onclick="openAuthModal('register')">Đăng Ký</button>
        `;
    }
}

async function loadCategories() {
    try {
        const res = await API.getCategories();
        if (res.success && res.data) {
            const select = document.getElementById('filter-category');
            res.data.forEach(cat => {
                const opt = document.createElement('option');
                opt.value = cat.id;
                opt.textContent = `${cat.name} (${cat.tourCount})`;
                select.appendChild(opt);
            });
        }
    } catch (err) {
        console.error('Không tải được danh mục:', err);
    }
}

async function loadTours(params = {}) {
    const container = document.getElementById('tours-container');
    container.innerHTML = '<div style="grid-column: 1/-1; text-align: center; padding: 2rem;">Đang tải danh sách tour...</div>';

    try {
        const res = await API.getTours(params);
        if (res.success && res.data) {
            if (res.data.length === 0) {
                container.innerHTML = '<div style="grid-column: 1/-1; text-align: center; padding: 2rem;">Không tìm thấy tour phù hợp tiêu chí.</div>';
                return;
            }

            container.innerHTML = res.data.map(tour => `
                <div class="tour-card">
                    <img src="${tour.imageUrl}" alt="${tour.title}" class="tour-img" onerror="this.src='https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800'">
                    <div class="tour-body">
                        <span class="tour-badge">${tour.categoryName || 'Tour Du Lịch'}</span>
                        <h3 class="tour-title">${tour.title}</h3>
                        <div class="tour-meta">
                            <span>📍 ${tour.destinationName}</span>
                            <span>⏱ ${tour.durationDays}N${tour.durationNights}Đ</span>
                        </div>
                        <p style="font-size: 0.85rem; color: var(--dark-muted); margin-bottom: 1rem;">${tour.summary || ''}</p>
                        <div class="tour-footer">
                            <div class="tour-price">
                                ${(tour.discountPrice || tour.price).toLocaleString('vi-VN')} đ
                                <small>/ khách</small>
                            </div>
                            <button class="btn btn-primary btn-sm" onclick="openBookingModal(${tour.id}, '${tour.title.replace(/'/g, "\\'")}', ${tour.discountPrice || tour.price})">Đặt Ngay</button>
                        </div>
                    </div>
                </div>
            `).join('');
        }
    } catch (err) {
        container.innerHTML = `<div style="grid-column: 1/-1; text-align: center; padding: 2rem; color: var(--danger);">Lỗi tải tour: ${err.message}</div>`;
    }
}

function searchTours() {
    const keyword = document.getElementById('filter-keyword').value.trim();
    const categoryId = document.getElementById('filter-category').value;
    const params = {};
    if (keyword) params.keyword = keyword;
    if (categoryId) params.categoryId = categoryId;
    loadTours(params);
}

// Modal handling
function openModal(id) {
    document.getElementById(id).classList.add('active');
}

function closeModal(id) {
    document.getElementById(id).classList.remove('active');
}

function openAuthModal(tab = 'login') {
    switchAuthTab(tab);
    openModal('auth-modal');
}

function switchAuthTab(tab) {
    const loginForm = document.getElementById('login-form');
    const registerForm = document.getElementById('register-form');
    const title = document.getElementById('auth-modal-title');

    if (tab === 'login') {
        loginForm.style.display = 'block';
        registerForm.style.display = 'none';
        title.textContent = 'Đăng Nhập Tài Khoản';
    } else {
        loginForm.style.display = 'none';
        registerForm.style.display = 'block';
        title.textContent = 'Đăng Ký Tài Khoản Mới';
    }
}

function quickFill(email, password) {
    document.getElementById('login-email').value = email;
    document.getElementById('login-password').value = password;
}

async function handleLogin(e) {
    e.preventDefault();
    const email = document.getElementById('login-email').value.trim();
    const password = document.getElementById('login-password').value;

    try {
        const res = await API.login(email, password);
        showToast('Đăng nhập thành công!', 'success');
        closeModal('auth-modal');
        initAuthUI();
        if (res.data.user.role === 'ADMIN') {
            showToast('Bạn đã đăng nhập với quyền ADMIN tối cao!', 'info');
        }
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function handleRegister(e) {
    e.preventDefault();
    const fullName = document.getElementById('reg-fullname').value.trim();
    const email = document.getElementById('reg-email').value.trim();
    const phone = document.getElementById('reg-phone').value.trim();
    const password = document.getElementById('reg-password').value;

    try {
        await API.register(fullName, email, password, phone);
        showToast('Đăng ký tài khoản thành công và đã tự động đăng nhập!', 'success');
        closeModal('auth-modal');
        initAuthUI();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function handleLogout() {
    await API.logout();
    initAuthUI();
    showToast('Đã đăng xuất tài khoản thành công.', 'info');
}

// Booking Handling
function openBookingModal(tourId, tourTitle, pricePerPerson) {
    const user = TokenStorage.getUser();
    if (!user) {
        showToast('Vui lòng đăng nhập để tiến hành đặt tour.', 'error');
        openAuthModal('login');
        return;
    }

    document.getElementById('book-tour-id').value = tourId;
    document.getElementById('book-tour-title').value = tourTitle;
    document.getElementById('book-price-per-person').value = pricePerPerson;

    document.getElementById('book-name').value = user.fullName || '';
    document.getElementById('book-email').value = user.email || '';
    document.getElementById('book-phone').value = user.phoneNumber || '';

    calculateBookingTotal();
    openModal('booking-modal');
}

function calculateBookingTotal() {
    const price = parseFloat(document.getElementById('book-price-per-person').value) || 0;
    const adults = parseInt(document.getElementById('book-adults').value) || 1;
    const children = parseInt(document.getElementById('book-children').value) || 0;

    const total = (adults * price) + (children * price * 0.75);
    document.getElementById('book-total-display').textContent = total.toLocaleString('vi-VN') + ' đ';
}

async function handleCreateBooking(e) {
    e.preventDefault();

    const tourId = parseInt(document.getElementById('book-tour-id').value);
    const tourTitle = document.getElementById('book-tour-title').value;
    const price = parseFloat(document.getElementById('book-price-per-person').value);
    const name = document.getElementById('book-name').value.trim();
    const email = document.getElementById('book-email').value.trim();
    const phone = document.getElementById('book-phone').value.trim();
    const adults = parseInt(document.getElementById('book-adults').value);
    const children = parseInt(document.getElementById('book-children').value);

    try {
        const res = await API.createBooking({
            tourId,
            tourTitle,
            departureId: 1,
            startDate: new Date(Date.now() + 7 * 86400000).toISOString(),
            contactName: name,
            contactEmail: email,
            contactPhone: phone,
            adultsCount: adults,
            childrenCount: children,
            pricePerPerson: price
        });

        closeModal('booking-modal');
        showToast('Đặt tour thành công!', 'success');

        // Hiển thị mã QR VietQR
        document.getElementById('qr-image').src = res.data.vietQrUrl;
        document.getElementById('qr-booking-code').textContent = res.data.bookingCode;
        document.getElementById('qr-amount').textContent = res.data.finalAmount.toLocaleString('vi-VN') + ' đ';
        openModal('qr-modal');

    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function openMyBookingsModal() {
    openModal('my-bookings-modal');
    const tbody = document.getElementById('my-bookings-tbody');
    tbody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Đang tải...</td></tr>';

    try {
        const res = await API.getMyBookings();
        if (res.success && res.data) {
            if (res.data.length === 0) {
                tbody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Bạn chưa có đơn đặt tour nào.</td></tr>';
                return;
            }

            tbody.innerHTML = res.data.map(b => `
                <tr>
                    <td><strong>${b.bookingCode}</strong></td>
                    <td>${b.tourTitle}</td>
                    <td>${b.adultsCount} NL, ${b.childrenCount} TE</td>
                    <td style="color: var(--danger); font-weight: bold;">${b.finalAmount.toLocaleString('vi-VN')} đ</td>
                    <td>
                        <span class="badge ${b.status === 'CONFIRMED' ? 'badge-success' : (b.status === 'CANCELLED' ? 'badge-danger' : 'badge-warning')}">
                            ${b.status}
                        </span>
                    </td>
                    <td>
                        ${b.status === 'PENDING' ? `<button class="btn btn-danger btn-sm" onclick="cancelUserBooking(${b.id})">Hủy</button>` : '-'}
                    </td>
                </tr>
            `).join('');
        }
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="6" style="text-align: center; color: var(--danger);">${err.message}</td></tr>`;
    }
}

async function cancelUserBooking(id) {
    if (!confirm('Bạn có chắc chắn muốn hủy đơn đặt tour này?')) return;
    try {
        await API.cancelBooking(id);
        showToast('Hủy đơn đặt tour thành công.', 'info');
        openMyBookingsModal();
    } catch (err) {
        showToast(err.message, 'error');
    }
}
