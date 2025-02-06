package com.db.fusion.rs.stax;

import java.io.FileOutputStream;
import java.io.OutputStream;
import java.sql.*;
import javax.xml.stream.XMLOutputFactory;
import javax.xml.stream.XMLStreamWriter;

public class QuorumXMLGenerator {

    // Database connection details
    private static final String URL = "jdbc:oracle:thin:@(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCPS)(HOST=frqrasu.de.da)))";
    private static final String USER = "DCA_Y2ZN_USER";
    private static final String PASSWORD = "DC_Tue291347599May";

    // SQL Queries
    private static final String SQL_RESTRICTION_TYPE =
            "SELECT DISTINCT v.restriction_type_name FROM quorum_ing_owner.v_dmart_restriction v WHERE v.restriction_type_cd = ? AND v.status_cde = ? AND ROWNUM <= 1";

    private static final String SQL_RESTRICTIONS =
            "SELECT DISTINCT v.restriction_id, v.instrument_name, v.instrument_id_type, "
                    + "CASE WHEN v.instrument_id_type = ? THEN SUBSTR(v.instrument_id_value, 1, 14) ELSE v.instrument_id_value END AS instrument_id_value "
                    + "FROM quorum_ing_owner.v_dmart_restriction v WHERE v.restriction_type_cd = ? AND v.status_cde = ? "
                    + "ORDER BY v.restriction_id";

    public static void main(String[] args) {
        try (Connection connection = DriverManager.getConnection(URL, USER, PASSWORD);
             OutputStream outputStream = new FileOutputStream("restricted_securities.xml")) {

            // Initialize XML writer
            XMLOutputFactory factory = XMLOutputFactory.newInstance();
            XMLStreamWriter xtw = factory.createXMLStreamWriter(outputStream, "UTF-8");

            // Start XML document
            xtw.writeStartDocument("UTF-8", "1.0");
            xtw.writeStartElement("restrictedSecurityList");

            // Write report details dynamically from database
            writeReportDetails(connection, xtw);

            // Write restriction type dynamically from database
            fetchRestrictionType(connection, xtw);

            // Write restrictions dynamically from database
            fetchRestrictions(connection, xtw);

            // Close the root element
            xtw.writeEndElement();
            xtw.writeEndDocument();
            xtw.flush();
            xtw.close();

            System.out.println("XML file generated successfully!");

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    // Method to dynamically write report details from the database
    private static void writeReportDetails(Connection connection, XMLStreamWriter xtw) throws Exception {
        // Get the report details from the database or generate it dynamically
        String restrictionType = "Tender/Exercise voting rights";  // Example static value (can be fetched dynamically)
        String timestamp = "2024-11-19T16:00:07";  // Example static value (can be fetched dynamically)
        String status = "Success";  // Example static value (can be fetched dynamically)

        xtw.writeStartElement("reportDetails");

        xtw.writeStartElement("typeOfRestriction");
        xtw.writeCharacters(restrictionType);
        xtw.writeEndElement();

        xtw.writeStartElement("extractTimestamp");
        xtw.writeCharacters(timestamp);
        xtw.writeEndElement();

        xtw.writeStartElement("extractStatus");
        xtw.writeCharacters(status);
        xtw.writeEndElement();

        xtw.writeEndElement(); // Close reportDetails
    }

    // Method to fetch restriction type dynamically from the database
    private static void fetchRestrictionType(Connection connection, XMLStreamWriter xtw) throws SQLException {
        try (PreparedStatement ps = connection.prepareStatement(SQL_RESTRICTION_TYPE)) {
            ps.setString(1, "20");  // Restriction Type Code
            ps.setString(2, "I");   // Status Code

            ResultSet rs = ps.executeQuery();
            while (rs.next()) {
                xtw.writeStartElement("restrictionType");
                xtw.writeCharacters(rs.getString(1)); // Dynamically fetched restriction type
                xtw.writeEndElement();
            }
        }
    }

    // Method to fetch restrictions dynamically from the database
    private static void fetchRestrictions(Connection connection, XMLStreamWriter xtw) throws SQLException {
        try (PreparedStatement ps = connection.prepareStatement(SQL_RESTRICTIONS)) {
            ps.setString(1, "Debt");  // Instrument ID Type
            ps.setString(2, "20");    // Restriction Type Code
            ps.setString(3, "I");     // Status Code

            ResultSet rs = ps.executeQuery();
            while (rs.next()) {
                xtw.writeStartElement("restrictedSecurity");

                // Dynamically write security description
                xtw.writeStartElement("securityDescription");
                xtw.writeCharacters(rs.getString(2)); // Dynamically fetched instrument name
                xtw.writeEndElement();

                // Dynamically create security identifiers
                createSecurityIdentifier(xtw, "ISIN", rs.getString(1)); // Dynamically fetched instrument ISIN
                createSecurityIdentifier(xtw, "WPK", rs.getString(1)); // Dynamically fetched WPK (if available)

                xtw.writeEndElement(); // Close restrictedSecurity
            }
        }
    }

    // Helper method to create dynamic securityIdentifier elements
    private static void createSecurityIdentifier(XMLStreamWriter xtw, String code, String value) throws Exception {
        xtw.writeStartElement("securityIdentifier");

        xtw.writeStartElement("security:securityNumberingAgencyCode");
        xtw.writeCharacters(code); // Dynamically fetched code (e.g., ISIN, WPK)
        xtw.writeEndElement();

        xtw.writeStartElement("security:securityIdentifier");
        xtw.writeCharacters(value); // Dynamically fetched value
        xtw.writeEndElement();

        xtw.writeEndElement(); // Close securityIdentifier
    }
}
