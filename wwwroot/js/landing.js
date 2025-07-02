// Landing Page JavaScript Functions

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    initializeSearchFeatures();
    initializeFilterFeatures();
    initializeCarCardAnimations();
    initializeScrollEffects();
    initializeLazyLoading();
    initializeTooltips();
});

// Search Features
function initializeSearchFeatures() {
    const searchInput = document.querySelector('input[name="query"]');
    if (searchInput) {
        // Add search suggestions
        searchInput.addEventListener('input', debounce(handleSearchInput, 300));

        // Add keyboard shortcuts
        document.addEventListener('keydown', function (e) {
            // Ctrl/Cmd + K to focus search
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                searchInput.focus();
            }
        });
    }
}

// Filter Features
function initializeFilterFeatures() {
    // Auto-submit filter form when dropdown changes
    const filterSelects = document.querySelectorAll('.filter-section select');
    filterSelects.forEach(select => {
        select.addEventListener('change', function () {
            // Add loading indicator
            showFilterLoading();

            // Submit form after short delay for better UX
            setTimeout(() => {
                this.closest('form').submit();
            }, 300);
        });
    });

    // Price range formatter
    const priceInputs = document.querySelectorAll('input[name="minHarga"], input[name="maxHarga"]');
    priceInputs.forEach(input => {
        input.addEventListener('input', function () {
            formatPriceInput(this);
        });
    });
}

// Car Card Animations
function initializeCarCardAnimations() {
    const carCards = document.querySelectorAll('.car-card');

    // Add hover effects
    carCards.forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-8px)';
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
        });
    });

    // Add intersection observer for fade-in animation
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1 });

    carCards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(card);
    });
}

// Scroll Effects
function initializeScrollEffects() {
    // Smooth scroll for anchor links
    const anchorLinks = document.querySelectorAll('a[href^="#"]');
    anchorLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);

            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Navbar background on scroll
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 50) {
                navbar.classList.add('navbar-scrolled');
            } else {
                navbar.classList.remove('navbar-scrolled');
            }
        });
    }

    // BACK-TO-TOP BUTTON DIHAPUS - TIDAK ADA LAGI
}

// Lazy Loading for Images
function initializeLazyLoading() {
    const images = document.querySelectorAll('img[data-src]');

    const imageObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('loading');
                imageObserver.unobserve(img);
            }
        });
    });

    images.forEach(img => {
        img.classList.add('loading');
        imageObserver.observe(img);
    });
}

// Initialize Bootstrap Tooltips
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Utility Functions

// Debounce function for search input
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Handle search input with suggestions
function handleSearchInput(event) {
    const query = event.target.value.trim();

    if (query.length >= 2) {
        // You can implement search suggestions here
        console.log('Searching for:', query);
    }
}

// Show loading indicator for filters
function showFilterLoading() {
    const filterButton = document.querySelector('.filter-section button[type="submit"]');
    if (filterButton) {
        const originalText = filterButton.innerHTML;
        filterButton.innerHTML = '<span class="loading"></span> Memfilter...';
        filterButton.disabled = true;

        // Reset after form submission
        setTimeout(() => {
            filterButton.innerHTML = originalText;
            filterButton.disabled = false;
        }, 1000);
    }
}

// Format price input
function formatPriceInput(input) {
    let value = input.value.replace(/\D/g, '');
    if (value) {
        value = parseInt(value).toLocaleString('id-ID');
        input.setAttribute('data-original', input.value);
        // Don't format while typing to avoid cursor issues
    }
}

// FUNGSI createBackToTopButton() DIHAPUS SEPENUHNYA

// Price Calculator for Car Details Page
function calculateLoanPayment(price, downPaymentPercent, months) {
    const downPayment = price * (downPaymentPercent / 100);
    const loanAmount = price - downPayment;
    const interestRate = 0.12 / 12; // 12% annual interest rate
    const monthlyPayment = loanAmount * (interestRate * Math.pow(1 + interestRate, months)) / (Math.pow(1 + interestRate, months) - 1);

    return {
        downPayment: downPayment,
        monthlyPayment: monthlyPayment,
        totalInterest: (monthlyPayment * months) - loanAmount
    };
}

// Format currency for Indonesian Rupiah
function formatCurrency(amount) {
    return new Intl.NumberFormat('id-ID', {
        style: 'currency',
        currency: 'IDR',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    }).format(amount);
}

// WhatsApp Message Builder
function buildWhatsAppMessage(carName, carPrice, action = 'inquiry') {
    const messages = {
        inquiry: `Halo, saya tertarik dengan mobil ${carName} seharga ${formatCurrency(carPrice)}. Bisa minta informasi lebih detail?`,
        testDrive: `Halo, saya ingin jadwal test drive untuk mobil ${carName}. Kapan waktu yang tersedia?`,
        negotiation: `Halo, saya tertarik dengan ${carName}. Apakah harganya bisa dinegosiasi?`
    };

    return encodeURIComponent(messages[action] || messages.inquiry);
}

// Image Error Handler
function handleImageError(img) {
    img.src = '/images/placeholder-car.png';
    img.alt = 'Gambar tidak tersedia';
    img.style.backgroundColor = '#f8f9fa';
}

// Add error handlers to all car images
document.addEventListener('DOMContentLoaded', function () {
    const carImages = document.querySelectorAll('.car-image');
    carImages.forEach(img => {
        img.addEventListener('error', function () {
            handleImageError(this);
        });
    });
});

// Search Analytics (Optional)
function trackSearch(query) {
    // You can implement analytics tracking here
    console.log('Search tracked:', query);

    // Example: Send to Google Analytics
    if (typeof gtag !== 'undefined') {
        gtag('event', 'search', {
            search_term: query
        });
    }
}

// Filter Analytics (Optional)
function trackFilter(filterType, filterValue) {
    console.log(`Filter tracked: ${filterType} = ${filterValue}`);

    // Example: Send to Google Analytics
    if (typeof gtag !== 'undefined') {
        gtag('event', 'filter', {
            filter_type: filterType,
            filter_value: filterValue
        });
    }
}

// Car View Analytics (Optional)
function trackCarView(carId, carName) {
    console.log(`Car view tracked: ${carId} - ${carName}`);

    // Example: Send to Google Analytics
    if (typeof gtag !== 'undefined') {
        gtag('event', 'view_item', {
            item_id: carId,
            item_name: carName,
            item_category: 'car'
        });
    }
}

// Add CSS for navbar scroll effect - BACK-TO-TOP CSS DIHAPUS
const style = document.createElement('style');
style.textContent = `
    .navbar-scrolled {
        background-color: rgba(255, 255, 255, 0.95) !important;
        backdrop-filter: blur(10px);
        box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    }
    
    .loading {
        filter: blur(5px);
        transition: filter 0.3s;
    }
`;
document.head.appendChild(style);