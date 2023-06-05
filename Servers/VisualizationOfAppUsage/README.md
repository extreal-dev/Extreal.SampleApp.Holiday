# Visualization fo App Usage

Grafanaのダッシュボードを準備します。
Docker Composeを使ってGrafanaを起動し、ブラウザでGrafanaアクセスしてダッシュボードを使います。

1. Docker ComposeでGrafanaを起動します。
    ```
    $ docker-compose up -d
    ```
1. しばらくしてからサービスが起動したかを確認します。STATUSが全てhealthyになっていればOKです。
    ```
    $ docker-compose ps
    ```
    unhealthyになっているサービスがある場合は個別に再起動します。
    ```
    $ docker-compose restart <<service>>
    ```
1. ブラウザでlocalhost:3000を指定してGrafanaにアクセスします。
1. Sign inを選択します。
    - username: admin
    - password: admin
    - adminの新しいパスワードを設定します。
1. Grafanaのトップページが表示されます。左側のメニューアイコンからDashboardsを選択します。
1. 右側のNewプルダウンからImportを選択します。
    1. このREADMEと同じ場所にあるholiday-dashboard.jsonをアップロードします。
    1. LokiプルダウンからLokiを選択します。
    1. Importを選択します。
1. ダッシュボードが表示されれば準備完了です。

