﻿using Footballers.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto;

[XmlType("Footballer")]
public class ExportFootballerDto
{
    [XmlElement("Name")]
    public string Name { get; set; } = null!;

    [XmlElement("Position")]
    public string Position { get; set; } = null!;
}
