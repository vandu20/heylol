import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import java.sql.*;

@SpringBootApplication
public class QuorumDatabaseApplication implements CommandLineRunner {

    private static final String URL = "jdbc:oracle:thin:@your_quorum_host:1521:your_db";
    private static final String USER = "your_username";
    private static final String PASSWORD = "your_password";

    private static final String SQL_RESTRICTION_TYPE = "SELECT DISTINCT v." + ExtractorConstants.RESTRICTION_TYPE_NAME +
            " FROM " + ExtractorConstants.VW_RESTRICTION + " v WHERE v." + ExtractorConstants.RESTRICTION_TYPE_CODE + " = ?" +
            " AND ROWNUM <= 1 AND " + ExtractorConstants.STATUS_CODE + " = ?";

    private static final String SQL_RESTRICTIONS = "SELECT DISTINCT v." + ExtractorConstants.RESTRICTION_ID +
            ", v." + ExtractorConstants.SECURITY_NAME +
            ", v." + ExtractorConstants.INSTRUMENT_ID_TYPE +
            ", (CASE WHEN v." + ExtractorConstants.INSTRUMENT_ID_TYPE + " = ?" +
            " THEN SUBSTR(v." + ExtractorConstants.INSTRUMENT_ID_VALUE + ", 1, " + ExtractorConstants.RIC_MAX_SIZE + ")" +
            " ELSE v." + ExtractorConstants.INSTRUMENT_ID_VALUE + " END) AS " + ExtractorConstants.INSTRUMENT_ID_VALUE +
            " FROM (SELECT * FROM (SELECT DISTINCT t." + ExtractorConstants.INSTRUMENT_ISIN + " AS " + ExtractorConstants.RESTRICTION_ID +
            ", t." + ExtractorConstants.INSTRUMENT_ISIN + ", " + ExtractorConstants.INSTRUMENT_CUSIP +
            ", t." + ExtractorConstants.INSTRUMENT_WKN + ", " + ExtractorConstants.INSTRUMENT_RIC +
            ", t." + ExtractorConstants.SECURITY_NAME +
            " FROM " + ExtractorConstants.VW_RESTRICTION + " t" +
            " WHERE t." + ExtractorConstants.STATUS_CODE + " = ?" +
            " AND t." + ExtractorConstants.INSTRUMENT_ISIN + " IS NOT NULL" +
            " AND t." + ExtractorConstants.RESTRICTION_TYPE_CODE + " = ?" +
            ") UNPIVOT(" + ExtractorConstants.INSTRUMENT_ID_VALUE + " FOR " + ExtractorConstants.INSTRUMENT_ID_TYPE +
            " IN (" + ExtractorConstants.INSTRUMENT_ISIN + " AS '" + ExtractorConstants.ISIN + "', " +
            ExtractorConstants.INSTRUMENT_CUSIP + " AS '" + ExtractorConstants.CUSIP + "', " +
            ExtractorConstants.INSTRUMENT_WKN + " AS '" + ExtractorConstants.WPK + "', " +
            ExtractorConstants.INSTRUMENT_RIC + " AS '" + ExtractorConstants.RIC + "'))) v" +
            " ORDER BY " + ExtractorConstants.RESTRICTION_ID;

    public static void main(String[] args) {
        SpringApplication.run(QuorumDatabaseApplication.class, args);
    }

    @Override
    public void run(String... args) throws Exception {
        try (Connection connection = DriverManager.getConnection(URL, USER, PASSWORD)) {
            fetchRestrictionType(connection);
            fetchRestrictions(connection);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    private void fetchRestrictionType(Connection connection) throws SQLException {
        try (PreparedStatement preparedStatement = connection.prepareStatement(SQL_RESTRICTION_TYPE)) {
            preparedStatement.setString(1, "some_code"); // Set restriction type code
            preparedStatement.setString(2, "some_status"); // Set status code
            ResultSet resultSet = preparedStatement.executeQuery();
            while (resultSet.next()) {
                System.out.println("Restriction Type: " + resultSet.getString(1));
            }
        }
    }

    private void fetchRestrictions(Connection connection) throws SQLException {
        try (PreparedStatement preparedStatement = connection.prepareStatement(SQL_RESTRICTIONS)) {
            preparedStatement.setString(1, "some_type");
            preparedStatement.setString(2, "some_status");
            preparedStatement.setString(3, "some_restriction_type");
            ResultSet resultSet = preparedStatement.executeQuery();
            while (resultSet.next()) {
                System.out.println("Restriction ID: " + resultSet.getString(1) + ", Security Name: " + resultSet.getString(2));
            }
        }
    }
}
