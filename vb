import javax.xml.stream.XMLStreamException;
import javax.xml.stream.XMLStreamWriter;
import javax.xml.stream.XMLOutputFactory;
import java.sql.ResultSet;
import java.sql.SQLException;

public class XmlGenerator {

    public static void generateXml(ResultSet resultSet) throws XMLStreamException, SQLException {
        // Create XMLStreamWriter
        XMLOutputFactory outputFactory = XMLOutputFactory.newInstance();
        XMLStreamWriter writer = outputFactory.createXMLStreamWriter(System.out);

        // Start XML document
        writer.writeStartDocument("UTF-8", "1.0");
        writer.writeStartElement("crestrictedSecurityList");
        writer.writeAttribute("xmlns", "http://fusion.ibit.gto.db.com/FusionCDM/RestrictedSecurityList-v1");

        // Add reportDetails element
        writer.writeStartElement("reportDetails");

        writer.writeStartElement("typeOfRestriction");
        writer.writeCharacters("Tender/Exercise voting rights");
        writer.writeEndElement(); // typeOfRestriction

        writer.writeStartElement("extractTimestamp");
        writer.writeCharacters("2024-11-19T16:00:07");
        writer.writeEndElement(); // extractTimestamp

        writer.writeStartElement("extractStatus");
        writer.writeCharacters("Success");
        writer.writeEndElement(); // extractStatus

        writer.writeEndElement(); // reportDetails

        // Process each restriction and add to XML
        while (resultSet.next()) {
            String restrictionId = resultSet.getString("RESTRICTION_ID");
            String securityName = resultSet.getString("SECURITY_NAME");

            // Add restrictedSecurity element
            writer.writeStartElement("restrictedSecurity");

            // Add securityDescription
            writer.writeStartElement("securityDescription");
            writer.writeCharacters(securityName);  // Using the security name from the database
            writer.writeEndElement(); // securityDescription

            // Add securityIdentifier
            writer.writeStartElement("securityIdentifier");

            // ISIN identifier
            writer.writeStartElement("security:securityNumberingAgencyCode");
            writer.writeCharacters("ISIN");
            writer.writeEndElement(); // security:securityNumberingAgencyCode

            writer.writeStartElement("security:securityIdentifier");
            writer.writeCharacters("DE000HW6T156");  // This can be dynamic based on your database results
            writer.writeEndElement(); // security:securityIdentifier

            // WPK identifier
            writer.writeStartElement("security:securityNumberingAgencyCode");
            writer.writeCharacters("WPK");
            writer.writeEndElement(); // security:securityNumberingAgencyCode

            writer.writeStartElement("security:securityIdentifier");
            writer.writeCharacters("HW6T1S");  // This can be dynamic based on your database results
            writer.writeEndElement(); // security:securityIdentifier

            writer.writeEndElement(); // securityIdentifier
            writer.writeEndElement(); // restrictedSecurity
        }

        // End root element and document
        writer.writeEndElement(); // crestrictedSecurityList
        writer.writeEndDocument();

        // Close the writer
        writer.flush();
        writer.close();
    }
}
