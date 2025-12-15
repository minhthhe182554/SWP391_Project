(() => {
    const metricsCanvas = document.getElementById("metricsChart");
    const categoriesCanvas = document.getElementById("categoriesChart");
    const toggleButtons = Array.from(document.querySelectorAll("[data-days]"));
    const quickStats = {
        newUsers: document.querySelector('[data-stat="newUsers"]'),
        activeJobs: document.querySelector('[data-stat="activeJobs"]'),
        newApplications: document.querySelector('[data-stat="newApplications"]'),
        activeCompanies: document.querySelector('[data-stat="activeCompanies"]')
    };

    let metricsChart = null;
    let categoriesChart = null;
    let currentDays = 7;

    const colors = {
        newUsers: "#0d6efd",
        activeJobs: "#fd7e14",
        newApplications: "#198754",
        activeCompanies: "#6f42c1",
        bar: "#20c997"
    };

    const setActiveToggle = (days) => {
        toggleButtons.forEach((btn) => {
            btn.classList.toggle("active", parseInt(btn.dataset.days, 10) === days);
        });
    };

    const renderQuickStats = (data) => {
        const lastIndex = (data?.labels?.length ?? 0) - 1;
        if (lastIndex < 0) {
            Object.values(quickStats).forEach((el) => el && (el.textContent = "-"));
            return;
        }

        const assign = (key, series) => {
            if (!quickStats[key]) return;
            const value = Array.isArray(series) && series.length > lastIndex ? series[lastIndex] : 0;
            quickStats[key].textContent = value?.toString() ?? "0";
        };

        assign("newUsers", data.newUsers);
        assign("activeJobs", data.activeJobs);
        assign("newApplications", data.newApplications);
        assign("activeCompanies", data.activeCompanies);
    };

    const renderCharts = (data) => {
        if (!metricsCanvas || !categoriesCanvas || !window.Chart) return;

        const labels = data?.labels ?? [];
        const metricsDatasets = [
            {
                label: "Người dùng mới",
                data: data?.newUsers ?? [],
                borderColor: colors.newUsers,
                backgroundColor: "rgba(13, 110, 253, 0.12)",
                tension: 0.25,
                fill: true,
                pointRadius: 3
            },
            {
                label: "Công việc đang hoạt động",
                data: data?.activeJobs ?? [],
                borderColor: colors.activeJobs,
                backgroundColor: "rgba(253, 126, 20, 0.12)",
                tension: 0.25,
                fill: true,
                pointRadius: 3
            },
            {
                label: "Ứng tuyển mới",
                data: data?.newApplications ?? [],
                borderColor: colors.newApplications,
                backgroundColor: "rgba(25, 135, 84, 0.12)",
                tension: 0.25,
                fill: true,
                pointRadius: 3
            },
            {
                label: "Công ty hoạt động",
                data: data?.activeCompanies ?? [],
                borderColor: colors.activeCompanies,
                backgroundColor: "rgba(111, 66, 193, 0.12)",
                tension: 0.25,
                fill: true,
                pointRadius: 3
            }
        ];

        if (metricsChart) metricsChart.destroy();
        metricsChart = new Chart(metricsCanvas, {
            type: "line",
            data: { labels, datasets: metricsDatasets },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: { mode: "index", intersect: false },
                scales: {
                    y: { beginAtZero: true, ticks: { precision: 0 } }
                }
            }
        });

        const topCategories = data?.topJobCategories ?? [];
        const categoryLabels = topCategories.map((c) => c.category);
        const categoryCounts = topCategories.map((c) => c.applications);

        if (categoriesChart) categoriesChart.destroy();
        categoriesChart = new Chart(categoriesCanvas, {
            type: "bar",
            data: {
                labels: categoryLabels,
                datasets: [
                    {
                        label: "Ứng tuyển",
                        data: categoryCounts,
                        backgroundColor: colors.bar
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: { beginAtZero: true, ticks: { precision: 0 } }
                }
            }
        });

        renderQuickStats(data);
    };

    const fetchMetrics = async (days) => {
        try {
            const response = await fetch(`/Admin/DashboardMetrics?days=${days}`, {
                headers: { "Accept": "application/json" }
            });
            if (!response.ok) {
                throw new Error(`Fetch metrics failed with status ${response.status}`);
            }
            const payload = await response.json();
            renderCharts(payload);
        }
        catch (error) {
            console.error(error);
        }
    };

    const bootstrap = () => {
        const initial = window.initialDashboardData;
        if (initial && initial.labels?.length) {
            renderCharts(initial);
        } else {
            fetchMetrics(currentDays);
        }

        toggleButtons.forEach((btn) => {
            btn.addEventListener("click", () => {
                const days = parseInt(btn.dataset.days ?? "7", 10);
                currentDays = Number.isNaN(days) ? 7 : days;
                setActiveToggle(currentDays);
                fetchMetrics(currentDays);
            });
        });

        setActiveToggle(currentDays);
    };

    document.addEventListener("DOMContentLoaded", bootstrap);
})();

