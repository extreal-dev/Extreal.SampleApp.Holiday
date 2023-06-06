# Visualization fo App Usage

Prepare a Grafana dashboard.
Start Grafana using Docker Compose and use the dashboard with Grafana access in your browser.

1. Start Grafana with Docker Compose.
    ```
    $ docker-compose up -d
    ```
1. After a few moments, check to see if the service has started, and if STATUS is all healthy, it is OK.
    ```
    $ docker-compose ps
    ```
    If any services are unhealthy, restart them individually.
    ```
    $ docker-compose restart <<service>>
    ```
1. Access Grafana by specifying localhost:3000 in your browser.
1. Select Sign in.
    - username: admin
    - password: admin
    - Set a new password for admin.
1. The Grafana home page will appear. Select Dashboards from the menu icons on the left.
1. Select Import from the New pull-down on the right.
    1. Upload holiday-dashboard.json in the same location as this README.
    1. Select Loki from the Loki pull-down menu.
    1. Select Import.
1. Once the dashboard appears, you are ready to go.

