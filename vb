spring.datasource.url=jdbc:oracle:thin:@(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCPS)(HOST=your-db-host)(PORT=your-db-port))(CONNECT_DATA=(SERVICE_NAME=your-db-service)))
spring.datasource.username=DCA_Y2ZN_USER
spring.datasource.password=DC_Tue291347599May
spring.datasource.driver-class-name=oracle.jdbc.OracleDriver
spring.datasource.hikari.maximum-pool-size=10

# JPA settings (if using Hibernate)
spring.jpa.database-platform=org.hibernate.dialect.OracleDialect
spring.jpa.hibernate.ddl-auto=none
spring.jpa.show-sql=true
