import java.sql.*;
import javax.xml.stream.XMLStreamWriter;
import java.util.List;
import java.util.ArrayList;
import javax.ejb.EJB;
import javax.ejb.Stateful;

@Stateful
public class RSReportGenerator {

    private static final String SEPARATOR = ",";
    
    @EJB
    private RSDao rsDao;  // Assuming rsDao is your DAO class handling DB operations

    // Database connection details
    private static final String DB_URL = "jdbc:oracle:thin:@localhost:1521:xe"; // Replace with your DB URL
    private static final String DB_USER = "your_db_user"; // Replace with your DB username
    private static final String DB_PASSWORD = "your_db_password"; // Replace with your DB password
    
    // SQL Queries
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

    // Method to fetch restriction type from the database
    public String getRestrictionType(String restrictionCode) {
        try (Connection connection = DriverManager.getConnection(DB_URL, DB_USER, DB_PASSWORD);
             PreparedStatement stmt = connection.prepareStatement(SQL_RESTRICTION_TYPE)) {
            stmt.setString(1, restrictionCode);
            stmt.setString(2, ExtractorConstants.STATUS_ACTIVE);

            ResultSet resultSet = stmt.executeQuery();
            if (resultSet.next()) {
                return resultSet.getString(ExtractorConstants.RESTRICTION_TYPE_NAME);
            }
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return null;
    }

    // Method to fetch restrictions from the database
    public List<RestrictedSecurity> getRestrictions(String restrictionType) {
        List<RestrictedSecurity> restrictions = new ArrayList<>();
        try (Connection connection = DriverManager.getConnection(DB_URL, DB_USER, DB_PASSWORD);
             PreparedStatement stmt = connection.prepareStatement(SQL_RESTRICTIONS)) {
            stmt.setString(1, ExtractorConstants.INSTRUMENT_ID_ISIN);
            stmt.setString(2, ExtractorConstants.STATUS_ACTIVE);
            stmt.setString(3, restrictionType);

            ResultSet resultSet = stmt.executeQuery();
            while (resultSet.next()) {
                String restrictionId = resultSet.getString(ExtractorConstants.RESTRICTION_ID);
                String securityName = resultSet.getString(ExtractorConstants.SECURITY_NAME);
                String instrumentIdType = resultSet.getString(ExtractorConstants.INSTRUMENT_ID_TYPE);
                String instrumentIdValue = resultSet.getString(ExtractorConstants.INSTRUMENT_ID_VALUE);

                // Build RestrictedSecurity object
                RestrictedSecurity restrictedSecurity = new RestrictedSecurity(securityName);
                restrictedSecurity.addSecurityIdentifier(instrumentIdType, instrumentIdValue);
                restrictions.add(restrictedSecurity);
            }
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return restrictions;
    }

    // Method to generate the XML report
    public void generateReport(XMLStreamWriter xtw) throws Exception {
        String restrictionType = "SampleRestriction"; // Example restriction code
        String restrictionTypeName = getRestrictionType(restrictionType);
        List<RestrictedSecurity> restrictions = getRestrictions(restrictionType);

        StaXBuilder staXBuilder = new StaXBuilder(xtw);

        // Create report details
        staXBuilder.startElement(ExtractorConstants.REPORT_DETAILS)
                .addElement(ExtractorConstants.TYPE_OF_RESTRICTION, restrictionTypeName)
                .addElement(ExtractorConstants.EXTRACT_TIMESTAMP, DateTimeHelper.formatCalendarGmt(DateTimeHelper.getGMTCalendar()))
                .addElement(ExtractorConstants.EXTRACT_STATUS, "Success");

        // Start restricted security list
        staXBuilder.startElement(ExtractorConstants.RESTRICTED_SECURITY_LIST);
        for (RestrictedSecurity rsec : restrictions) {
            staXBuilder.startElement(ExtractorConstants.RESTRICTED_SECURITY)
                    .addElement(ExtractorConstants.SECURITY_DESCRIPTION, rsec.getSecurityDescription());

            // Loop through identifiers (ISIN, CUSIP, RIC, WPK)
            for (SecurityIdentifier id : rsec.getSecurityIdentifiers()) {
                staXBuilder.startElement(ExtractorConstants.SECURITY_IDENTIFIER)
                        .addElement(ExtractorConstants.SECURITY_NUMBERING_AGENCY_CODE, id.getType())
                        .addElement(ExtractorConstants.SECURITY_IDENTIFIER, id.getValue())
                        .endElement();
            }
            staXBuilder.endElement(); // End restricted security
        }
        staXBuilder.endElement(); // End restricted security list

        staXBuilder.endElement(); // End report details
    }
}
