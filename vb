package com.db.fusion.instruments.ondemand.util;

import com.db.fusion.instruments.ondemand.constants.ExtractorConstants;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

import java.sql.*;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.*;

@SpringBootApplication
public class QuorumDatabaseApplication implements CommandLineRunner {

    private static final String URL = "jdbc:oracle:thin:@(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCPS)(HOST=frqrasu.de.da";
    private static final String USER = "DCA_Y2ZN_USER";
    private static final String PASSWORD = "DC_Tue291347599May";
    
    private static final String SQL_RESTRICTION_TYPE = "SELECT DISTINCT v." + ExtractorConstants.RESTRICTION_TYPE_NAME +
            " FROM " + ExtractorConstants.VW_RESTRICTION + " WHERE v." + ExtractorConstants.RESTRICTION_TYPE_CODE + "= ? " +
            "AND ROWNUM <= 1 AND " + ExtractorConstants.STATUS_CODE + " = ?";

    private static final String SQL_RESTRICTIONS = "SELECT DISTINCT v." + ExtractorConstants.RESTRICTION_ID + ", " +
            "v." + ExtractorConstants.SECURITY_NAME + ", " +
            "v." + ExtractorConstants.INSTRUMENT_ID_TYPE + ", " +
            "(CASE WHEN v." + ExtractorConstants.INSTRUMENT_ID_TYPE + " = ? THEN " +
            "substr(v." + ExtractorConstants.INSTRUMENT_ID_VALUE + ", 1, " + ExtractorConstants.RIC_MAX_SIZE + ") " +
            "ELSE v." + ExtractorConstants.INSTRUMENT_ID_VALUE + " END) AS " + ExtractorConstants.INSTRUMENT_ID +
            " FROM " + ExtractorConstants.VW_RESTRICTION + " v " +
            "WHERE v." + ExtractorConstants.STATUS_CODE + " = ? " +
            "AND v." + ExtractorConstants.RESTRICTION_TYPE_CODE + " = ? " +
            "AND v." + ExtractorConstants.INSTRUMENT_ISIN + " IS NOT NULL";

    public static void main(String[] args) {
        SpringApplication.run(QuorumDatabaseApplication.class, args);
    }

    @Override
    public void run(String... args) throws Exception {

        try (Connection connection = DriverManager.getConnection(URL, USER, PASSWORD)) {

            // Create a new Document for XML generation
            DocumentBuilderFactory docFactory = DocumentBuilderFactory.newInstance();
            DocumentBuilder docBuilder = docFactory.newDocumentBuilder();
            Document doc = docBuilder.newDocument();

            // Create root element
            Element rootElement = doc.createElementNS("http://fusion.ibit.gto.db.com/FusionCOM/RestrictedSecurityList-v1", "restrictedSecurityList");
            doc.appendChild(rootElement);

            // Create reportDetails element
            Element reportDetails = doc.createElement("reportDetails");
            rootElement.appendChild(reportDetails);

            Element typeOfRestriction = doc.createElement("typeOfRestriction");
            typeOfRestriction.appendChild(doc.createTextNode("Tender/Exercise voting rights"));
            reportDetails.appendChild(typeOfRestriction);

            Element extractTimestamp = doc.createElement("extractTimestamp");
            extractTimestamp.appendChild(doc.createTextNode("2024-11-19T16:00:07"));
            reportDetails.appendChild(extractTimestamp);

            Element extractStatus = doc.createElement("extractStatus");
            extractStatus.appendChild(doc.createTextNode("Success"));
            reportDetails.appendChild(extractStatus);

            fetchRestrictions(connection, doc, rootElement);

            // Output the generated XML
            System.out.println("Generated XML:");
            printXML(doc);

        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    private void fetchRestrictions(Connection connection, Document doc, Element rootElement) throws SQLException {
        try (PreparedStatement preparedStatement = connection.prepareStatement(SQL_RESTRICTIONS)) {
            preparedStatement.setString(1, "Debt");
            preparedStatement.setString(2, "I");
            preparedStatement.setString(3, "14");

            ResultSet resultSet = preparedStatement.executeQuery();

            while (resultSet.next()) {
                // Create restrictedSecurity element for each row
                Element restrictedSecurity = doc.createElement("restrictedSecurity");
                rootElement.appendChild(restrictedSecurity);

                Element securityDescription = doc.createElement("securityDescription");
                securityDescription.appendChild(doc.createTextNode(resultSet.getString(2))); // Security Name
                restrictedSecurity.appendChild(securityDescription);

                // Create securityIdentifier elements for each security type (ISIN, WPK, etc.)
                createSecurityIdentifier(doc, restrictedSecurity, "ISIN", resultSet.getString(1)); // Instrument ISIN
                createSecurityIdentifier(doc, restrictedSecurity, "WPK", resultSet.getString(1)); // Placeholder for other identifiers

            }

        }
    }

    private void createSecurityIdentifier(Document doc, Element restrictedSecurity, String code, String value) {
        Element securityIdentifier = doc.createElement("securityIdentifier");
        restrictedSecurity.appendChild(securityIdentifier);

        Element agencyCode = doc.createElement("security:securityNumberingAgencyCode");
        agencyCode.appendChild(doc.createTextNode(code));
        securityIdentifier.appendChild(agencyCode);

        Element identifier = doc.createElement("security:securityIdentifier");
        identifier.appendChild(doc.createTextNode(value));
        securityIdentifier.appendChild(identifier);
    }

    private void printXML(Document doc) throws Exception {
        // Print XML as string (for debugging or logging)
        TransformerFactory transformerFactory = TransformerFactory.newInstance();
        Transformer transformer = transformerFactory.newTransformer();
        DOMSource source = new DOMSource(doc);
        StreamResult result = new StreamResult(System.out);
        transformer.transform(source, result);
    }
}
